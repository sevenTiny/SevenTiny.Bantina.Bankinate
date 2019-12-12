using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Caching;
using System.ComponentModel;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.Caching
{
    public class LocalQueryCacheTest
    {
        [DataBase("SevenTinyTest")]
        private class LocalQueryCache : MySqlDbContext<LocalQueryCache>
        {
            public LocalQueryCache() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
            {
                this.OpenLocalCache(true, false);
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        public void Add()
        {
            using (var db = new LocalQueryCache())
            {
                //先查询肯定是没有的
                db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.False(db.IsFromCache);

                //第二次在缓存中可以查到
                db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.True(db.IsFromCache);

                //Add操作会清空一级缓存
                db.Add(new OperationTest { });

                //这时候查询应该从缓存获取不到
                db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.False(db.IsFromCache);
            }
        }

        [Fact]
        public void QueryAll()
        {
            using (var db = new LocalQueryCache())
            {
                db.Queryable<OperationTest>().ToList();

                Assert.False(db.IsFromCache);

                db.Queryable<OperationTest>().ToList();

                Assert.True(db.IsFromCache);
            }
        }

        [Fact]
        public void QueryCount()
        {
            using (var db = new LocalQueryCache())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                Assert.False(db.IsFromCache);

                var result = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                Assert.True(db.IsFromCache);

                Assert.Equal(0, result);
            }
        }

        [Fact]
        public void QueryWhereWithUnSameCondition()
        {
            using (var db = new LocalQueryCache())
            {
                db.Queryable<OperationTest>().Where(t => t.Id == 1).FirstOrDefault();

                Assert.False(db.IsFromCache);

                db.Queryable<OperationTest>().Where(t => t.Id == 2).FirstOrDefault();

                Assert.False(db.IsFromCache);
            }
        }

        [Fact]
        [Description("设置缓存过期时间进行测试")]
        public void QueryCacheExpired()
        {
            using (var db = new LocalQueryCache())
            {
                db.Queryable<OperationTest>().Where(t => t.Id == 1).FirstOrDefault();

                Assert.False(db.IsFromCache);

                //未完成/。。。。。。。。。。。




                db.Queryable<OperationTest>().Where(t => t.Id == 1).FirstOrDefault();

                Assert.False(db.IsFromCache);
            }
        }

        [Fact]
        [Description("两次查出来的结果不正确【由于内存做的缓存，改内存数据时缓存会一起变动...作为缓存时，慎改内存数据】")]
        public void QueryBugRepaire2()
        {
            int metaObjectId = 1;
            using (var db = new LocalQueryCache())
            {
                for (int i = 0; i < 3; i++)
                {
                    var re = db.Queryable<OperationTest>().Where(t => t.IntNullKey == 1 && t.IntKey == metaObjectId).ToList();
                    Assert.NotNull(re);
                }
            }
        }
    }
}
