using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Bankinate.Helpers.Redis;
using System;
using Newtonsoft.Json;

namespace SevenTiny.Bantina.Bankinate.CacheManagement
{
    /// <summary>
    /// 缓存存储管理器
    /// </summary>
    internal class CacheStorageManager
    {
        internal DbContext DbContext;

        internal CacheStorageManager(DbContext context)
        {
            DbContext = context;
        }

        private static IRedisCache redisCache = null;
        private static IRedisCache GetRedisCacheProvider(DbContext context)
        {
            if (redisCache != null)
                return redisCache;

            //配置的异常是参数异常，在处理数据时应抛出异常，其他连接异常应该忽略返回获取缓存失败
            if (string.IsNullOrEmpty(context.CacheMediaServer))
                throw new ArgumentException("Cache server address error", "dbContext.CacheMediaServer");

            redisCache = new RedisCacheManager(context.CacheMediaServer);

            if (redisCache == null)
                throw new Exception("redis init timeout");

            return redisCache;
        }

        public bool IsExist(string key)
        {
            return IsExist(key, out object obj);
        }
        public bool IsExist<TValue>(string key, out TValue value)
        {
            switch (DbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Exist(key, out value);
                case CacheMediaType.Redis:
                    try
                    {
                        var redisResult = GetRedisCacheProvider(DbContext).Get(key);
                        if (!string.IsNullOrEmpty(redisResult))
                        {
                            value = JsonConvert.DeserializeObject<TValue>(redisResult);
                            return true;
                        }
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    value = default(TValue);
                    return false;
                default:
                    value = default(TValue);
                    return false;

            }
        }
        public void Put<T>(string key, T value, TimeSpan expiredTime)
        {
            switch (DbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Put(key, value, expiredTime);
                    break;
                case CacheMediaType.Redis:
                    try
                    {
                        GetRedisCacheProvider(DbContext).Set(key, JsonConvert.SerializeObject(value), expiredTime);
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    break;
                default:
                    break;
            }
        }
        public T Get<T>(string key)
        {
            switch (DbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Get<string, T>(key);
                case CacheMediaType.Redis:
                    try
                    {
                        var redisResult = GetRedisCacheProvider(DbContext).Get(key);
                        if (!string.IsNullOrEmpty(redisResult))
                        {
                            return JsonConvert.DeserializeObject<T>(redisResult);
                        }
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    return default(T);
                default:
                    return default(T);
            }
        }
        public void Delete(string key)
        {
            switch (DbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Delete<string>(key);
                    break;
                case CacheMediaType.Redis:
                    try
                    {
                        GetRedisCacheProvider(DbContext).Delete(key);
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
