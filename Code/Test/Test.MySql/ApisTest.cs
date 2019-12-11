using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        [Description("持久化测试")]
        public void Persistence()
        {
            using (var db = new ApiDb())
            {
                int value = 999999;

                //初次查询没有数据
                var re = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re);

                //add一条数据
                OperationTest model = new OperationTest
                {
                    IntKey = value,
                    StringKey = "AddTest"
                };
                model.IntKey = value;
                db.Add<OperationTest>(model);

                //插入后查询有一条记录
                var re1 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Single(re1);
                Assert.Equal(value, re1.First().IntKey);

                //查询一条
                var entity = db.Queryable<OperationTest>().Where(t => t.IntKey == value).FirstOrDefault();
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
                db.Update<OperationTest>(entity);

                var entity2 = db.Queryable<OperationTest>().Where(t => t.IntKey == value).FirstOrDefault();
                Assert.NotNull(entity2);
                Assert.Equal(value, entity2.IntNullKey);
                Assert.Equal($"UpdateTest_{value}", entity2.StringKey);

                //删除数据
                db.Delete<OperationTest>(t => t.IntKey == value);

                //删除后查询没有
                var re4 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re4);
            }
        }

        [Fact]
        [Description("持久化测试_默认使用实体主键删除数据")]
        public void Persistence_DeleteEntity()
        {
            using (var db = new ApiDb())
            {
                int value = 999999;

                //初次查询没有数据
                var re = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re);

                //add一条数据
                OperationTest model = new OperationTest
                {
                    IntKey = value,
                    StringKey = "AddTest"
                };
                model.IntKey = value;
                db.Add<OperationTest>(model);

                //插入后查询有一条记录
                var re1 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Single(re1);
                Assert.Equal(value, re1.First().IntKey);

                var entity = db.Queryable<OperationTest>().Where(t => t.IntKey == value).FirstOrDefault();
                Assert.NotNull(entity);
                Assert.Equal(value, entity.IntKey);

                //删除数据
                db.Delete<OperationTest>(entity);

                //删除后查询没有
                var re4 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
                Assert.Null(re4);
            }
        }

        [Fact]
        public void Add()
        {
            using (var db = new ApiDb())
            {
                db.Add(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("INSERT INTO OperateTest (Key2,StringKey,IntKey,IntNullKey,DateNullKey,DateTimeNullKey,DoubleNullKey) VALUES (@Key2,@StringKey,@IntKey,@IntNullKey,@DateNullKey,@DateTimeNullKey,@DoubleNullKey)", db.SqlStatement);
                Assert.Equal(new[] { "@Key2", "@StringKey", "@IntKey", "@IntNullKey", "@DateNullKey", "@DateTimeNullKey", "@DoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void AddAsync()
        {
            using (var db = new ApiDb())
            {
                var a = db.AddAsync(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("INSERT INTO OperateTest (Key2,StringKey,IntKey,IntNullKey,DateNullKey,DateTimeNullKey,DoubleNullKey) VALUES (@Key2,@StringKey,@IntKey,@IntNullKey,@DateNullKey,@DateTimeNullKey,@DoubleNullKey)", db.SqlStatement);
                Assert.Equal(new[] { "@Key2", "@StringKey", "@IntKey", "@IntNullKey", "@DateNullKey", "@DateTimeNullKey", "@DoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Update()
        {
            using (var db = new ApiDb())
            {
                db.Update(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("UPDATE OperateTest t SET Key2=@tKey2,StringKey=@tStringKey,IntKey=@tIntKey,IntNullKey=@tIntNullKey,DateNullKey=@tDateNullKey,DateTimeNullKey=@tDateTimeNullKey,DoubleNullKey=@tDoubleNullKey WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId", "@tKey2", "@tStringKey", "@tIntKey", "@tIntNullKey", "@tDateNullKey", "@tDateTimeNullKey", "@tDoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void UpdateAsync()
        {
            using (var db = new ApiDb())
            {
                var a = db.UpdateAsync(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("UPDATE OperateTest t SET Key2=@tKey2,StringKey=@tStringKey,IntKey=@tIntKey,IntNullKey=@tIntNullKey,DateNullKey=@tDateNullKey,DateTimeNullKey=@tDateTimeNullKey,DoubleNullKey=@tDoubleNullKey WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId", "@tKey2", "@tStringKey", "@tIntKey", "@tIntNullKey", "@tDateNullKey", "@tDateTimeNullKey", "@tDoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Delete()
        {
            using (var db = new ApiDb())
            {
                db.Delete(new OperationTest
                {
                    Id = 1
                });

                Assert.Equal("DELETE t FROM OperateTest t WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void DeleteAsync()
        {
            using (var db = new ApiDb())
            {
                var a = db.DeleteAsync(new OperationTest
                {
                    Id = 1
                });

                Assert.Equal("DELETE t FROM OperateTest t WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Query_All()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE  1=1", db.SqlStatement);
                Assert.Equal(new string[0], db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Query_Where()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_MultiWhere()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("3")).Where(t => t.IntKey == 3).ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE (( 1=1 )  AND  (t.StringKey LIKE @tStringKey))  AND  (t.IntKey = @tIntKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey", "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3%", "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Select()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).ToList();
                Assert.Equal("SELECT t.IntKey,t.StringKey FROM OperateTest t  WHERE ( 1=1 )  AND  (t.IntKey <= @tIntKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_OrderBy()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).ToList();
                Assert.Equal("SELECT t.IntKey,t.StringKey FROM OperateTest t  WHERE ( 1=1 )  AND  (t.IntKey <= @tIntKey)  ORDER BY t.IntKey DESC", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Limit()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.IntKey > 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Limit(30).ToList();
                Assert.Equal(30, re.Count);
            }
        }

        [Fact]
        public void Query_Paging()
        {
            using (var db = new ApiDb())
            {
                var re4 = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(0, 10).ToList();
                var re5 = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Paging(0, 10).ToList();
                var re6 = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(1, 10).ToList();
                Assert.True(re4.Count == re5.Count && re5.Count == re6.Count && re6.Count == re4.Count);
            }
        }

        [Fact]
        public void Query_Any()
        {
            using (var db = new ApiDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).Any();
                //db.SqlStatement = "SELECT COUNT(0) FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)";
                Assert.True(re);
            }
        }
    }
}