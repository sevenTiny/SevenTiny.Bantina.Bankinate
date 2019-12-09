using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate.Core
{
    /// <summary>
    /// 对象操作实体集合
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbSet<TEntity> : ILinqQueryable<TEntity> where TEntity : class
    {
        public DbSet(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 操作上下文
        /// </summary>
        public DbContext DbContext { get; set; }

        /// <summary>
        /// 获取查询提供器
        /// </summary>
        private ILinqQueryable<TEntity> Queryable => QueryEngineSelector.Select<TEntity>(DbContext.DataBaseType, DbContext);

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Limit(int count)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Paging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns)
        {
            throw new NotImplementedException();
        }

        public object ToData()
        {
            throw new NotImplementedException();
        }

        public DataSet ToDataSet()
        {
            throw new NotImplementedException();
        }

        public List<TEntity> ToList()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }
}
