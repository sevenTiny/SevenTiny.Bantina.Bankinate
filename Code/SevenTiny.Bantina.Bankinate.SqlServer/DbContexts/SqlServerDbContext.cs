using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.SqlServer.SqlStatementManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class SqlServerDbContext<TDataBase> : SqlDbContext, IDisposable where TDataBase : class
    {
        protected SqlServerDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {
            DataBaseType = DataBaseType.SqlServer;
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }

        internal override void CreateDbConnection(string connectionString)
        {
            DbConnection = new SqlConnection(connectionString);
        }
        internal override void CreateDbCommand()
        {
            DbCommand = new SqlCommand();
            DbCommand.Connection = this.DbConnection;
        }
        internal override void CreateDbDataAdapter()
        {
            DbDataAdapter = new SqlDataAdapter();
            DbDataAdapter.SelectCommand = this.DbCommand;
        }
        internal override void CreateCommandTextGenerator() => CommandTextGenerator = new SqlServerCommandTextGenerator(this);
        /// <summary>
        /// 命令参数初始化
        /// </summary>
        internal override void ParameterInitializes()
        {
            if (Parameters != null && Parameters.Any())
            {
                DbCommand.Parameters.Clear();
                Parameters.Foreach(t => DbCommand.Parameters.Add(new SqlParameter(t.Key, t.Value ?? DBNull.Value)));
            }
        }

        internal override List<TEntity> GetFullCollectionData<TEntity>()
        {
            //多线程下使用同一个Connection会出现问题，这里采用新的连接进行异步数据操作
            using (var db = new TableCacheDbContext(this.ConnectionManager.ConnectionString_Write, this.ConnectionManager.ConnectionStrings_Read))
            {
                db.DataBaseName = this.DataBaseName;
                db.SqlStatement = $"SELECT * FROM {CollectionName}";
                return db.QueryExecutor.ExecuteList<TEntity>();
            }
        }

        /// <summary>
        /// 专门为扫描数据库表做的上下文
        /// </summary>
        private class TableCacheDbContext : SqlServerDbContext<TableCacheDbContext>
        {
            public TableCacheDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
            {
            }
        }

        public new void Dispose()
        {
            //调用基类的Dispose
            base.Dispose();
        }
    }
}
