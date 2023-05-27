using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PlayniteServices.IGDB;
using PlayniteServices.Discord;

namespace PlayniteServices;

public class Database
{
    private static bool instantiated = false;
    public readonly MongoClient MongoClient;
    public static readonly ReplaceOptions ItemUpsertOptions = new ReplaceOptions { IsUpsert = true };
    public static readonly InsertManyOptions InsertManyOptions = new InsertManyOptions();
    public readonly IMongoDatabase MongoDb;

    public readonly IMongoCollection<User> Users;
    public readonly IMongoCollection<AddonManifestBase> Addons;
    public readonly IMongoCollection<AddonInstallerManifestBase> AddonInstallers;
    public readonly IMongoCollection<AddonUpdateNotification> DiscordAddonNotifications;

    public Database(UpdatableAppSettings settings)
    {
        TestAssert.IsFalse(instantiated, $"{nameof(Database)} already instantiated");
        instantiated = true;

        // BSON doesn't support unsigned numbers so we need to use specific converter for ulong numbers
        BsonSerializer.RegisterSerializer(
            new UInt64Serializer(MongoDB.Bson.BsonType.Int64, new RepresentationConverter(true, true)));

        IgdbManager.RegisterClassMaps();

        BsonClassMap.RegisterClassMap<User>(cm =>
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

        var clientSettings = MongoClientSettings.FromConnectionString(settings.Settings.DatabaseConString);
        clientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
        MongoClient = new MongoClient(clientSettings);
        MongoDb = MongoClient.GetDatabase("playnitebackend");
        Users = MongoDb.GetCollection<User>("Users");
        Addons = MongoDb.GetCollection<AddonManifestBase>("Addons");
        AddonInstallers = MongoDb.GetCollection<AddonInstallerManifestBase>("AddonInstallers");
        DiscordAddonNotifications = MongoDb.GetCollection<AddonUpdateNotification>("DiscordAddonNotifications");
    }
}
