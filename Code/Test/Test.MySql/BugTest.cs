using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System.ComponentModel;
using System.Linq;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.MySql
{
    public class BugTest
    {
        [DataBase("SevenTinyTest")]
        private class BugDb : MySqlDbContext<BugDb>
        {
            public BugDb() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
            {
                //不真实持久化
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        [Description("修复同字段不同值的，sql和参数生成错误; 修复生成sql语句由于没有括号，逻辑顺序有误")]
        public void Query_BugRepaire1()
        {
            using (var db = new BugDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.IntKey == 1 && t.Id != 2 && (t.StringKey.Contains("1") || t.StringKey.Contains("2"))).FirstOrDefault();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE ( 1=1 )  AND  (((t.IntKey = @tIntKey)  AND  (t.Id <> @tId))  AND  ((t.StringKey LIKE @tStringKey)  Or  (t.StringKey LIKE @tStringKey0)))  LIMIT 1", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey", "@tId", "@tStringKey", "@tStringKey0" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "1", "2", "%1%", "%2%" }, db.Parameters.Values.ToArray());
            }
        }
    }
}
