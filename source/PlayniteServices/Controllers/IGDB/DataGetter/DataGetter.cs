using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PlayniteServices.Databases;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class DataGetter<T> where T : IgdbItem
    {
        public readonly string EndpointPath;
        internal readonly IgdbApi igdbApi;
        public IMongoCollection<T> Collection { get; private set; }

        public DataGetter(IgdbApi igdbApi, string endpointPath)
        {
            this.igdbApi = igdbApi;
            this.EndpointPath = endpointPath;

            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            Collection = Database.Instance.MongoDb.GetCollection<T>($"IGDB_col_{endpointPath}");
        }

        public virtual async Task<T> Get(ulong objectId)
        {
            return await igdbApi.GetItem(objectId, EndpointPath, Collection);
        }

        public virtual async Task<List<T>> Get(List<ulong> objectIds)
        {
            return await igdbApi.GetItem(objectIds, EndpointPath, Collection);
        }

        public virtual void Add(T item)
        {
            igdbApi.Add(item, Collection);
        }

        public virtual void Add(IEnumerable<T> items)
        {
            igdbApi.Add(items, Collection);
        }

        public virtual void DropCollection()
        {
            Database.Instance.MongoDb.DropCollection($"IGDB_col_{EndpointPath}");
            Collection = Database.Instance.MongoDb.GetCollection<T>($"IGDB_col_{EndpointPath}");
        }
    }
}
