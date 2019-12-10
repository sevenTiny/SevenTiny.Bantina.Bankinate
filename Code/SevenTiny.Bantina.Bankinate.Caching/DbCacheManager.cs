using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 数据库缓存管理器
    /// Bankinate的缓存是分为两级的，每级都有对应的开关
    /// 一级缓存（QueryCache查询缓存），缓存简短查询中的缓存数据
    /// 二级缓存（TableCache表缓存），缓存整张表，需要标签配合使用
    /// </summary>
    public class DbCacheManager : CacheManagerBase, IDbCacheManager
    {
        public DbCacheManager(DbContext context, CacheOptions cacheOptions) : base(context, cacheOptions)
        {
            Ensure.ArgumentNotNullOrEmpty(context, nameof(context));
            Ensure.ArgumentNotNullOrEmpty(cacheOptions, nameof(cacheOptions));

            if (cacheOptions.OpenQueryCache)
                QueryCacheManager = new QueryCacheManager(context, cacheOptions);
            if (cacheOptions.OpenTableCache)
                TableCacheManager = new TableCacheManager(context, cacheOptions);
        }

        internal QueryCacheManager QueryCacheManager { get; private set; }
        internal TableCacheManager TableCacheManager { get; private set; }

        /// 清空所有缓存
        /// </summary>
        public void FlushAllCache()
        {
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushAllCache();
            if (CacheOptions.OpenTableCache)
                TableCacheManager.FlushAllCache();
        }
        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        public void FlushCurrentCollectionCache(string collectionName = null)
        {
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache(collectionName);
            if (CacheOptions.OpenTableCache)
                TableCacheManager.FlushCollectionCache(collectionName);
        }

        public void Add<TEntity>(TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.AddCache(entity);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.AddCache(entities);
        }
        public void Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.UpdateCache(entity, filter);
        }
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.DeleteCache(filter);
        }
        public void Delete<TEntity>(TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.DeleteCache(entity);
        }

        public List<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func) where TEntity : class
        {
            List<TEntity> entities = null;

            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            if (CacheOptions.OpenTableCache)
                entities = TableCacheManager.GetEntitiesFromCache(filter);

            //2.判断是否在一级QueryCahe中
            if (CacheOptions.OpenQueryCache)
                if (entities == null || !entities.Any())
                    entities = QueryCacheManager.GetEntitiesFromCache<List<TEntity>>();

            //3.如果都没有，则直接从逻辑中获取
            if (entities == null || !entities.Any())
            {
                entities = func();
                DbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(entities);
            }

            return entities;
        }
        public TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> filter, Func<TEntity> func) where TEntity : class
        {
            TEntity result = null;

            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            if (CacheOptions.OpenTableCache)
                result = TableCacheManager.GetEntitiesFromCache(filter)?.FirstOrDefault();

            //2.判断是否在一级QueryCahe中
            if (CacheOptions.OpenQueryCache)
                if (result == null)
                    result = QueryCacheManager.GetEntitiesFromCache<TEntity>();

            //3.如果都没有，则直接从逻辑中获取
            if (result == null || result == default(TEntity))
            {
                result = func();
                DbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(result);
            }

            return result;
        }
        public long GetCount<TEntity>(Expression<Func<TEntity, bool>> filter, Func<long> func) where TEntity : class
        {
            long? result = null;

            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            if (CacheOptions.OpenTableCache)
                result = TableCacheManager.GetEntitiesFromCache(filter)?.Count;

            //2.判断是否在一级QueryCahe中
            if (CacheOptions.OpenQueryCache)
                if (result == null)
                    result = QueryCacheManager.GetEntitiesFromCache<long?>();

            //3.如果都没有，则直接从逻辑中获取
            if (result == null || result == default(long))
            {
                result = func();
                DbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(result);
            }

            return result ?? default(long);
        }
        public T GetObject<T>(Func<T> func) where T : class
        {
            T result = null;

            //1.判断是否在一级QueryCache中
            if (CacheOptions.OpenTableCache)
                result = QueryCacheManager.GetEntitiesFromCache<T>();

            //2.如果都没有，则直接从逻辑中获取
            if (result == null)
            {
                result = func();
                DbContext.IsFromCache = false;
                //3.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(result);
            }

            return result;
        }
    }
}
