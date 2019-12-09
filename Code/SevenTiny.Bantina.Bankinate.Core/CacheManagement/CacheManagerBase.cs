using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate.CacheManagement
{
    public abstract class CacheManagerBase
    {
        protected DbContext DbContext;

        protected CacheManagerBase(DbContext context)
        {
            DbContext = context;
            CacheStorageManager = new CacheStorageManager(context);
        }

        internal CacheStorageManager CacheStorageManager { get; set; }
    }
}
