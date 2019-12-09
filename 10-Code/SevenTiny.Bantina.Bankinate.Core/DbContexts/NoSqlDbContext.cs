using System;
using System.Runtime.CompilerServices;

//需要扩展的类型需要在此添加对应的程序集友元标识
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.MongoDb")]
namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class NoSqlDbContext : DbContext
    {
        protected NoSqlDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {
        }

        /// <summary>
        /// 一级缓存Key
        /// </summary>
        internal string QueryCacheKey { get; set; }

        internal override string GetQueryCacheKey()
        {
            return QueryCacheKey;
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
