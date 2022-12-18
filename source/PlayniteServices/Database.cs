using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Playnite;
using PlayniteServices.Models.Discord;
using PlayniteServices.Models.IGDB;
using PlayniteServices.Models.Steam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices
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

        private static bool instantiated = false;
        private readonly UpdatableAppSettings settings;
        private readonly MongoClient client;
        public static readonly ReplaceOptions ItemUpsertOptions = new ReplaceOptions { IsUpsert = true };
        public readonly IMongoDatabase MongoDb;

        public readonly IMongoCollection<Models.User> Users;
        public readonly IMongoCollection<SteamIdGame> SteamIgdbMatches;
        public readonly IMongoCollection<GameIdMatch> IGBDGameIdMatches;
        public readonly IMongoCollection<SearchIdMatch> IGDBSearchIdMatches;
        public readonly IMongoCollection<AddonManifestBase> Addons;
        public readonly IMongoCollection<AddonInstallerManifestBase> AddonInstallers;
        public readonly IMongoCollection<IgdbSearchResult> IgdbStdSearches;
        public readonly IMongoCollection<IgdbSearchResult> IgdbAltSearches;
        public readonly IMongoCollection<AddonUpdateNotification> DiscordAddonNotifications;

        public Database(UpdatableAppSettings settings)
        {
            TestAssert.IsFalse(instantiated, $"{nameof(Patreon)} already instantiated");
            instantiated = true;
            this.settings = settings;

            ConventionRegistry.Register(
                "Custom Conventions",
                new ConventionPack { new IgnoreDefaultPropertiesConvention() },
                t => t.FullName?.StartsWith("Playnite", StringComparison.Ordinal) == true);

            // BSON doesn't support unsigned numbers so we need to use specific converter for ulong numbers
            BsonSerializer.RegisterSerializer(
                new UInt64Serializer(MongoDB.Bson.BsonType.Int64, new RepresentationConverter(true, true)));

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

            BsonClassMap.RegisterClassMap<AddonInstallerManifestBase>(cm =>
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

            BsonClassMap.RegisterClassMap<AddonUpdateNotification>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.AddonId);
            });

            client = new MongoClient(settings.Settings.DatabaseConString);
            MongoDb = client.GetDatabase("playnitebackend");
            Users = MongoDb.GetCollection<Models.User>("Users");
            SteamIgdbMatches = MongoDb.GetCollection<SteamIdGame>("SteamIgdbMatches");
            IGBDGameIdMatches = MongoDb.GetCollection<GameIdMatch>("IGBDGameIdMatches");
            IGDBSearchIdMatches = MongoDb.GetCollection<SearchIdMatch>("IGDBSearchIdMatches");
            Addons = MongoDb.GetCollection<AddonManifestBase>("Addons");
            AddonInstallers = MongoDb.GetCollection<AddonInstallerManifestBase>("AddonInstallers");
            IgdbStdSearches = MongoDb.GetCollection<IgdbSearchResult>("Igdb_StdSearches");
            IgdbAltSearches = MongoDb.GetCollection<IgdbSearchResult>("Igdb_AltSearches");
            DiscordAddonNotifications = MongoDb.GetCollection<AddonUpdateNotification>("DiscordAddonNotifications");
        }

        public void Dispose()
        {
        }
    }
}
