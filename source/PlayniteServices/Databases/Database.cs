using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Playnite;
using PlayniteServices.Models.IGDB;
using PlayniteServices.Models.Steam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Databases
{
    public class Database : IDisposable
    {
        public class IgnoreDefaultPropertiesConvention : IMemberMapConvention
        {
            public string Name => "Ignore default properties.";
            public void Apply(BsonMemberMap mm)
            {
                mm.SetIgnoreIfDefault(true);
            }
        }

        private readonly MongoClient client;
        public static Database Instance { get; set; }
        public static readonly ReplaceOptions ItemUpsertOptions = new ReplaceOptions { IsUpsert = true };
        public readonly IMongoDatabase MongoDb;

        public readonly IMongoCollection<Models.User> Users;
        public readonly IMongoCollection<SteamIdGame> SteamIgdbMatches;
        public readonly IMongoCollection<GameIdMatch> IGBDGameIdMatches;
        public readonly IMongoCollection<SearchIdMatch> IGDBSearchIdMatches;
        public readonly IMongoCollection<AddonManifestBase> Addons;
        public readonly IMongoCollection<IgdbSearchResult> IgdbStdSearches;
        public readonly IMongoCollection<IgdbSearchResult> IgdbAltSearches;

        public Database(string connectionString)
        {
            ConventionRegistry.Register(
                "Custom Conventions",
                new ConventionPack { new IgnoreDefaultPropertiesConvention() },
                t => t.FullName.StartsWith("Playnite"));

            BsonClassMap.RegisterClassMap<IgdbItem>(cm => {
                cm.AutoMap();
                cm.MapIdMember(p => p.id);
                cm.SetIsRootClass(true);
            });

            BsonClassMap.RegisterClassMap<Models.User>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id);
            });

            BsonClassMap.RegisterClassMap<SteamIdGame>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.steamId);
            });

            BsonClassMap.RegisterClassMap<GameIdMatch>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id);
            });

            BsonClassMap.RegisterClassMap<SearchIdMatch>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id);
            });

            BsonClassMap.RegisterClassMap<AddonManifestBase>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.AddonId);
            });

            BsonClassMap.RegisterClassMap<IgdbSearchResult>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id);
            });

            client = new MongoClient(connectionString);
            MongoDb = client.GetDatabase("playnitebackend");
            Users = MongoDb.GetCollection<Models.User>("Users");
            SteamIgdbMatches = MongoDb.GetCollection<SteamIdGame>("SteamIgdbMatches");
            IGBDGameIdMatches = MongoDb.GetCollection<GameIdMatch>("IGBDGameIdMatches");
            IGDBSearchIdMatches = MongoDb.GetCollection<SearchIdMatch>("IGDBSearchIdMatches");
            Addons = MongoDb.GetCollection<AddonManifestBase>("Addons");
            IgdbStdSearches = MongoDb.GetCollection<IgdbSearchResult>("Igdb_StdSearches");
            IgdbAltSearches = MongoDb.GetCollection<IgdbSearchResult>("Igdb_AltSearches");
        }

        public void Dispose()
        {
        }
    }
}
