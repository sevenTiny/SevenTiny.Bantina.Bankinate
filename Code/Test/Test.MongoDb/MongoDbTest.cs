//using MongoDB.Bson;
//using MongoDB.Driver;
//using SevenTiny.Bantina.Bankinate;
//using SevenTiny.Bantina.Bankinate.Attributes;
//using System;
//using System.Collections.Generic;
//using Test.SevenTiny.Bantina.Bankinate.Helpers;
//using Test.SevenTiny.Bantina.Bankinate.Model;
//using Xunit;

//namespace Test.SevenTiny.Bantina.Bankinate.NoSqlDbTest
//{
//    [DataBase("local")]
//    public class MongoDb : MongoDbContext<MongoDb>
//    {
//        public MongoDb() : base(ConnectionStrings.Get("mongodb39911"))
//        {

//        }
//    }

//    public class MongoDbTest
//    {
//        [Fact]
//        public void GetConnectionString()
//        {
//            var conn = ConnectionStrings.Get("mongodb39911");
//        }

//        [Fact]
//        public void Add()
//        {
//            var bson = new BsonDocument
//                {
//                    { "name", $"7tiny_{1}" },
//                    { "age", 1 },
//                    { "sex", new Random(DateTime.Now.Millisecond).Next(3) },
//                };

//            using (MongoDb db = new MongoDb())
//            {
//                db.GetCollectionBson("TestCollection").InsertOne(bson);
//            }
//        }

//        [Fact]
//        public void Delete()
//        {
//            using (var db = new MongoDb())
//            {
//                db.Delete<Student>(t => true);
//            }
//        }

//        [Fact]
//        public void Search_Eq()
//        {
//            using (var db = new MongoDb())
//            {
//                var filter = Builders<BsonDocument>.Filter.Eq("name", "7tiny_9");
//                var result = db.QueryListBson<Student>(filter);
//            }
//        }

//        /// <summary>
//        /// 数组字段查询
//        /// </summary>
//        [Fact]
//        public void Search_AnyEq()
//        {
//            using (var db = new MongoDb())
//            {
//                //该例子不是一个数组
//                var filter = Builders<BsonDocument>.Filter.AnyEq("name", "7tiny_9");
//                var result = db.QueryListBson<Student>(filter);
//            }
//        }

//        /// <summary>
//        /// Less Than
//        /// </summary>
//        [Fact]
//        public void Search_Lt()
//        {
//            using (var db = new MongoDb())
//            {
//                var filter = Builders<BsonDocument>.Filter.Lt("age", 10);
//                var result = db.QueryListBson<Student>(filter);
//            }
//        }

//        /// <summary>
//        /// Greater Than
//        /// </summary>
//        [Fact]
//        public void Search_Gt()
//        {
//            using (var db = new MongoDb())
//            {
//                var filter = Builders<BsonDocument>.Filter.Gt("age", 30);
//                var result = db.QueryListBson<Student>(filter);
//            }
//        }
//    }
//}
