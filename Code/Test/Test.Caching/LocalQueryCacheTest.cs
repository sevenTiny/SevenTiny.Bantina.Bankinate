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

        [Theory]
        [InlineData(100)]
        public void QueryAll(int count)
        {
            using (var db = new LocalQueryCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperationTest>().ToList();

                    if (i == 0)
                        Assert.True(!db.IsFromCache);
                    else
                        Assert.True(db.IsFromCache);

                    Assert.Equal(1000, re.Count);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryOne(int count)
        {
            using (var db = new LocalQueryCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).FirstOrDefault();

                    if (i == 0)
                        Assert.True(!db.IsFromCache);
                    else
                        Assert.True(db.IsFromCache);

                    Assert.NotNull(re);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryCount(int count)
        {
            using (var db = new LocalQueryCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                    if (i == 0)
                        Assert.True(!db.IsFromCache);
                    else
                        Assert.True(db.IsFromCache);

                    Assert.Equal(1000, re);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryWhereWithUnSameCondition(int count)
        {
            using (var db = new LocalQueryCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperationTest>().Where(t => t.Id == 1).FirstOrDefault();
                    var re1 = db.Queryable<OperationTest>().Where(t => t.Id == 2).FirstOrDefault();

                    if (i == 0)
                        Assert.True(!db.IsFromCache);
                    else
                        Assert.True(db.IsFromCache);

                    Assert.NotEqual(re.StringKey, re1.StringKey);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryWhereWithUnSameCondition2(int count)
        {
            using (var db = new LocalQueryCache())
            {
                db.FlushCurrentCollectionCache(db.GetTableName<OperationTest>());

                for (int i = 1; i <= count; i++)
                {
                    var re = db.Queryable<OperationTest>().Where(t => t.Id == i).FirstOrDefault();

                    Assert.True(!db.IsFromCache);
                    Assert.NotNull(re);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        [Description("设置缓存过期时间进行测试")]
        public void QueryCacheExpired(int count)
        {
            using (var db = new LocalQueryCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).FirstOrDefault();
                    Assert.NotNull(re);
                }
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
