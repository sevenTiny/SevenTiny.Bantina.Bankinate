using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Caching;
using System;
using System.ComponentModel;
using System.Threading;
using Test.Common;
using Test.Common.Model;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Test.Caching
{
    [Collection("#1")]
    public class LocalTableCacheTest
    {
        /// <summary>
        /// 缓存秒
        /// </summary>
        private const int _CacheSecends = 1;

        [DataBase("SevenTinyTest")]
        private class LocalTableCache : MySqlDbContext<LocalTableCache>
        {
            public LocalTableCache() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
            {
                this.OpenLocalCache(openTableCache: true, tableCacheExpiredTimeSpan: TimeSpan.FromSeconds(_CacheSecends));
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        public void QueryAll()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                db.Queryable<OperationTest>().ToList();

                Assert.False(db.IsFromCache);

                Thread.CurrentThread.Join(1000);

                db.Queryable<OperationTest>().ToList();

                Assert.True(db.IsFromCache);
            }
        }

        [Fact]
        public void QueryCount()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                Assert.False(db.IsFromCache);

                var result = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                Assert.False(db.IsFromCache);

                Assert.Equal(0, result);
            }
        }

        [Fact]
        public void QueryWhereWithUnSameCondition()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

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
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                //先查询肯定是没有的
                db.Queryable<OperationTest>().Where(t => t.Id == 1).ToList();

                Assert.False(db.IsFromCache);

                Thread.CurrentThread.Join(1000);

                //第二次在缓存中可以查到
                db.Queryable<OperationTest>().Where(t => t.Id == 1).ToList();

                Assert.True(db.IsFromCache);

                Thread.Sleep(_CacheSecends * 1000);

                //这时候查询应该从缓存获取不到
                db.Queryable<OperationTest>().Where(t => t.Id == 1).ToList();

                Assert.False(db.IsFromCache);
            }
        }
    }
}
