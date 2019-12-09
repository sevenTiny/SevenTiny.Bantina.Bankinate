using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Bankinate
{
    public class MongoQueryable<TEntity> : NoSqlQueryable<TEntity> where TEntity : class
    {
        private IMongoDatabase _MongoDatabase;
        private FilterDefinition<TEntity> _filter;
        public MongoQueryable(NoSqlDbContext _dbContext, IMongoDatabase mongoDatabase) : base(_dbContext)
        {
            _MongoDatabase = mongoDatabase;
        }

        private IMongoCollection<TEntity> GetCollectionEntity()
        {
            return _MongoDatabase.GetCollection<TEntity>(TableAttribute.GetName(typeof(TEntity)));
        }

        public MongoQueryable<TEntity> WhereId(string _id)
        {
            if (!string.IsNullOrEmpty(_id))
                _filter = Builders<TEntity>.Filter.Eq("_id", _id);

            return this;
        }

        public override bool Any()
        {
            MustExistCheck();
            return Count() > 0;
        }

        public override long Count()
        {
            MustExistCheck();
            DbContext.QueryCacheKey = _where.ToString();
            return DbContext.DbCacheManager.GetCount(_where, () =>
            {
                return GetCollectionEntity().CountDocuments(_where);
            });
        }

        public override List<TEntity> ToList()
        {
            MustExistCheck();
            DbContext.QueryCacheKey = _where.ToString();
            return DbContext.DbCacheManager.GetEntities(_where, () =>
            {
                return GetCollectionEntity().Find(_where).ToList();
            });
        }

        public override TEntity ToOne()
        {
            //优先匹配id查询
            if (_filter != null)
            {
                DbContext.QueryCacheKey = _filter.ToString();
                return DbContext.DbCacheManager.GetEntity(_where, () =>
                {
                    return GetCollectionEntity().Find(_filter).SingleOrDefault();
                });
            }

            //降级匹配linq条件
            MustExistCheck();
            DbContext.QueryCacheKey = _where.ToString();
            return DbContext.DbCacheManager.GetEntity(_where, () =>
            {
                return GetCollectionEntity().Find(_where).SingleOrDefault();
            });
        }
    }
}
