using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
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
            }
        }

        [Fact]
        [Trait("bug", "修复同字段不同值的，sql和参数生成错误")]
        [Trait("bug", "修复生成sql语句由于没有括号，逻辑顺序有误")]
        public void Query_BugRepaire1()
        {
            using (var db = new BugDb())
            {
                var re = db.Queryable<OperateTestModel>().Where(t => t.IntKey == 1 && t.Id != 2 && (t.StringKey.Contains("1") || t.StringKey.Contains("2"))).FirstOrDefault();
                Assert.NotNull(re);
            }
        }
    }
}
