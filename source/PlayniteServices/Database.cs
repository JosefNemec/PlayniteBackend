using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Playnite;
using PlayniteServices.Controllers.IGDB;
using PlayniteServices.Models.Discord;

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
        public readonly MongoClient MongoClient;
        public static readonly ReplaceOptions ItemUpsertOptions = new ReplaceOptions { IsUpsert = true };
        public static readonly InsertManyOptions InsertManyOptions = new InsertManyOptions();
        public readonly IMongoDatabase MongoDb;

        public readonly IMongoCollection<Models.User> Users;
        public readonly IMongoCollection<AddonManifestBase> Addons;
        public readonly IMongoCollection<AddonInstallerManifestBase> AddonInstallers;
        public readonly IMongoCollection<AddonUpdateNotification> DiscordAddonNotifications;

        public Database(UpdatableAppSettings settings)
        {
            TestAssert.IsFalse(instantiated, $"{nameof(Database)} already instantiated");
            instantiated = true;

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

            BsonClassMap.RegisterClassMap<AddonUpdateNotification>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.AddonId);
            });

            MongoClient = new MongoClient(settings.Settings.DatabaseConString);
            MongoDb = MongoClient.GetDatabase("playnitebackend");
            Users = MongoDb.GetCollection<Models.User>("Users");
            Addons = MongoDb.GetCollection<AddonManifestBase>("Addons");
            AddonInstallers = MongoDb.GetCollection<AddonInstallerManifestBase>("AddonInstallers");
            DiscordAddonNotifications = MongoDb.GetCollection<AddonUpdateNotification>("DiscordAddonNotifications");
        }

        public void Dispose()
        {
        }
    }
}
