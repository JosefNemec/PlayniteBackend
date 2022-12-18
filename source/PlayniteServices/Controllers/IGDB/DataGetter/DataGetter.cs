using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class DataGetter<T> where T : IgdbItem
    {
        internal string endpointPath;
        internal IgdbApi igdbApi;
        public readonly IMongoCollection<T> Collection;

        public DataGetter(IgdbApi igdbApi, string endpointPath)
        {
            this.igdbApi = igdbApi;
            this.endpointPath = endpointPath;
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            Collection = igdbApi.Database.MongoDb.GetCollection<T>($"IGDB_col_{endpointPath}");
        }

        public virtual async Task<T?> Get(ulong objectId)
        {
            return await igdbApi.GetItem(objectId, endpointPath, Collection);
        }

        public virtual async Task<List<T>?> Get(List<ulong>? objectIds)
        {
            return await igdbApi.GetItem(objectIds, endpointPath, Collection);
        }
    }
}
