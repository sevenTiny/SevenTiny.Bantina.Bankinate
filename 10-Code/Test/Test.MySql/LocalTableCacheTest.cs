using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.MySql
{
    public class LocalTableCacheTest
    {
        [DataBase("SevenTinyTest")]
        private class LocalTableCache : MySqlDbContext<LocalTableCache>
        {
            public LocalTableCache() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
            {
                OpenTableCache = true;//二级缓存开关，表实体上的二级标签也需要提供
            }
        }

        [Trait("DESC","该方法和QueryAll放在一起可能冲突，分开进行执行单元测试")]
        [Fact]
        public void QueryAdd()
        {
            using (var db = new LocalTableCache())
            {
                //1.先查询肯定是没有的
                var re0 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.Null(re0);

                db.Add(new OperateTestModel
                {
                    IntKey = 123,
                    StringKey = "CacheAddTest123"
                });

                //2.这时候查询应该有一条，这次查询才加入缓存
                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.Single(re);

                //3.重复查询，这次是从缓存查的，还是一条
                var re2 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.Single(re2);

                //再次新增，清楚一级缓存
                db.Add(new OperateTestModel
                {
                    IntKey = 123,
                    StringKey = "CacheAddTest123"
                });

                //4.这次查应该从数据库查询，加入缓存，2条
                var re4 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.Equal(2, re4.Count);

                db.Delete<OperateTestModel>(t => t.StringKey.StartsWith("CacheAddTest"));

                //4.删除完毕以后，查询是没有的
                var re3 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
                Assert.Null(re3);
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryAll(int count)
        {
            using (var db = new LocalTableCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperateTestModel>().ToList();
                    Assert.Equal(1000, re.Count);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryOne(int count)
        {
            using (var db = new LocalTableCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("test")).FirstOrDefault();
                    Assert.NotNull(re);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryCount(int count)
        {
            using (var db = new LocalTableCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("test")).Count();
                    Assert.Equal(1000, re);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryWhereWithUnSameCondition(int count)
        {
            using (var db = new LocalTableCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperateTestModel>().Where(t => t.Id == 1).FirstOrDefault();
                    var re1 = db.Queryable<OperateTestModel>().Where(t => t.Id == 2).FirstOrDefault();
                    Assert.NotEqual(re.StringKey, re1.StringKey);
                }
            }
        }

        [Theory]
        [InlineData(100)]
        [Trait("desc", "设置缓存过期时间进行测试")]
        public void QueryCacheExpired(int count)
        {
            using (var db = new LocalTableCache())
            {
                for (int i = 0; i < count; i++)
                {
                    var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("test")).FirstOrDefault();
                    Assert.NotNull(re);
                }
            }
        }
    }
}
