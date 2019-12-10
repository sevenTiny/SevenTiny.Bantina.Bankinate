using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Caching;
using System;
using System.Linq;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.MySql
{
    /// <summary>
    /// 关系型数据库查询测试
    /// </summary>
    public class ApisTest
    {
        [DataBase("SevenTinyTest")]
        private class ApiDb : MySqlDbContext<ApiDb>
        {
            public ApiDb() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
            {
                RealExecutionSaveToDb = true;
            }
        }

        [Fact]
        [Trait("desc", "持久化测试")]
        public void Persistence()
        {
            using (var db = new ApiDb())
            {
                int value = 999999;

                //初次查询没有数据
                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re);

                //add一条数据
                OperateTestModel model = new OperateTestModel
                {
                    IntKey = value,
                    StringKey = "AddTest"
                };
                model.IntKey = value;
                db.Add<OperateTestModel>(model);

                //插入后查询有一条记录
                var re1 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Single(re1);
                Assert.Equal(value, re1.First().IntKey);

                //查询一条
                var entity = db.Queryable<OperateTestModel>().Where(t => t.IntKey == value).FirstOrDefault();
                Assert.NotNull(entity);
                Assert.Equal(value, entity.IntKey);

                //更新数据
                //entity.Id = value;   //自增的主键不应该被修改,如果用这种方式进行修改，给Id赋值就会导致修改不成功，因为条件是用第一个主键作为标识修改的
                entity.Key2 = value;
                entity.StringKey = $"UpdateTest_{value}";
                entity.IntNullKey = value;
                entity.DateTimeNullKey = DateTime.Now;
                entity.DateNullKey = DateTime.Now.Date;
                entity.DoubleNullKey = entity.IntNullKey;
                entity.FloatNullKey = entity.IntNullKey;
                db.Update<OperateTestModel>(entity);

                var entity2 = db.Queryable<OperateTestModel>().Where(t => t.IntKey == value).FirstOrDefault();
                Assert.NotNull(entity2);
                Assert.Equal(value, entity2.IntNullKey);
                Assert.Equal($"UpdateTest_{value}", entity2.StringKey);

                //删除数据
                db.Delete<OperateTestModel>(t => t.IntKey == value);

                //删除后查询没有
                var re4 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re4);
            }
        }

        [Fact]
        [Trait("desc", "持久化测试_默认使用实体主键删除数据")]
        public void Persistence_DeleteEntity()
        {
            using (var db = new ApiDb())
            {
                int value = 999999;

                //初次查询没有数据
                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re);

                //add一条数据
                OperateTestModel model = new OperateTestModel
                {
                    IntKey = value,
                    StringKey = "AddTest"
                };
                model.IntKey = value;
                db.Add<OperateTestModel>(model);

                //插入后查询有一条记录
                var re1 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Single(re1);
                Assert.Equal(value, re1.First().IntKey);

                var entity = db.Queryable<OperateTestModel>().Where(t => t.IntKey == value).FirstOrDefault();
                Assert.NotNull(entity);
                Assert.Equal(value, entity.IntKey);

                //删除数据
                db.Delete<OperateTestModel>(entity);

                //删除后查询没有
                var re4 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re4);
            }
        }

        [Fact]
        public void Query_All()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().ToList();
                Assert.Equal(1000, re.Count);
            }
        }

        [Fact]
        public void Query_Where()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.EndsWith("3")).ToList();
                Assert.Equal(100, re.Count);
            }
        }

        [Fact]
        public void Query_MultiWhere()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("3")).Where(t => t.IntKey == 3).ToList();
                Assert.Single(re);
            }
        }

        [Fact]
        public void Query_Select()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).ToList();
                Assert.Equal(3, re.Count);
            }
        }

        [Fact]
        public void Query_OrderBy()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.IntKey <= 9).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).ToList();
                Assert.True(re.Count == 9 && re.First().IntKey == 9 && re.First().Id == 0);//没有查id，id应该=0
            }
        }

        [Fact]
        public void Query_Limit()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.IntKey > 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Limit(30).ToList();
                Assert.Equal(30, re.Count);
            }
        }

        [Fact]
        public void Query_Paging()
        {
            using (var db = new ApiDb())
            {
                var re4 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(0, 10).ToList();
                var re5 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Paging(0, 10).ToList();
                var re6 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(1, 10).ToList();
                Assert.True(re4.Count == re5.Count && re5.Count == re6.Count && re6.Count == re4.Count);
            }
        }

        [Fact]
        public void Query_Any()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.EndsWith("3")).Any();
                //db.SqlStatement = "SELECT COUNT(0) FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)";
                Assert.True(re);
            }
        }
    }
}