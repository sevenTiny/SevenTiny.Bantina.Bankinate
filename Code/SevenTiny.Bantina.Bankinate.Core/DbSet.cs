using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.Core
{
    /// <summary>
    /// 对象操作实体集合,专用于强类型实体操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbSet<TEntity> : ILinqQueryable<TEntity> where TEntity : class
    {
        internal DbSet(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 操作上下文
        /// </summary>
        private DbContext DbContext { get; set; }

        /// <summary>
        /// 获取查询提供器
        /// </summary>
        private ILinqQueryable<TEntity> Queryable => QueryEngineSelector.Select<TEntity>(DbContext.DataBaseType, DbContext);

        public bool Any()
        {
            return Queryable.Any();
        }

        public long Count()
        {
            return Queryable.Count();
        }

        public TEntity FirstOrDefault()
        {
            return Queryable.FirstOrDefault();
        }

        public ILinqQueryable<TEntity> Limit(int count)
        {
            return Queryable.Limit(count);
        }

        public ILinqQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            return Queryable.OrderBy(orderBy);
        }

        public ILinqQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            return Queryable.OrderByDescending(orderBy);
        }

        public ILinqQueryable<TEntity> Paging(int pageIndex, int pageSize)
        {
            return Queryable.Paging(pageIndex, pageSize);
        }

        public ILinqQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns)
        {
            return Queryable.Select(columns);
        }

        public object ToData()
        {
            return Queryable.ToData();
        }

        public DataSet ToDataSet()
        {
            return Queryable.ToDataSet();
        }

        public List<TEntity> ToList()
        {
            return Queryable.ToList();
        }

        public ILinqQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            return Queryable.Where(filter);
        }
    }
}
