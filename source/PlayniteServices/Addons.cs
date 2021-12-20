using MongoDB.Bson;
using MongoDB.Driver;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Databases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class Addons : IDisposable
    {
        private readonly static object generatorLock = new object();
        private readonly static ILogger logger = LogManager.GetLogger();
        private readonly UpdatableAppSettings settings;
        private readonly Database db;
        private readonly HttpClient httpClient;
        private readonly System.Threading.Timer addonUpdatesTimer;

        public event EventHandler InstallerManifestsUpdated;

        public static Addons Instance { get; set; }

        public Addons(UpdatableAppSettings settings, Database db)
        {
            this.settings = settings;
            this.db = db;
            httpClient = new HttpClient { Timeout = new TimeSpan(0, 0, 20) };

            addonUpdatesTimer = new System.Threading.Timer(
                async (_) => await UpdateAddonInstallers(),
                null,
                new TimeSpan(0),
                new TimeSpan(0, 15, 0));
        }

        public void Dispose()
        {
            addonUpdatesTimer?.Dispose();
        }

        private async Task UpdateAddonInstallers()
        {
            logger.Info("Updating addon installers cache.");
            foreach (var addon in db.Addons.AsQueryable())
            {
                var installer = await GetInstallerManifest(addon);
                if (installer != null)
                {
                    db.AddonInstallers.ReplaceOne(
                        a => a.AddonId == installer.AddonId,
                        installer,
                        Database.ItemUpsertOptions);
                }
            }

            InstallerManifestsUpdated?.Invoke(this, null);
        }

        public AddonManifestBase GetAddon(string addonId)
        {
            var filter = Builders<AddonManifestBase>.Filter.Eq(u => u.AddonId, addonId);
            return db.Addons.Find(filter).FirstOrDefault();
        }

        public async Task<AddonInstallerManifestBase> GetInstallerManifest(AddonManifestBase addon)
        {
            try
            {
                var manifestStr = await httpClient.GetStringAsync(addon.InstallerManifestUrl);
                return Serialization.FromYaml<AddonInstallerManifestBase>(manifestStr);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to download addon installer manifest {addon.InstallerManifestUrl}");
                return null;
            }
        }

        public Task RegenerateAddonDatabase()
        {
            return Task.Factory.StartNew(() =>
            {
                logger.Info("Regenerating addons database.");
                lock (generatorLock)
                {
                    try
                    {
                        var col = Database.Instance.Addons;
                        col.DeleteMany(new BsonDocument());

                        var addonDirectory = settings.Settings.Addons.AddonRepository;
                        if (settings.Settings.Addons.AddonRepository.IsHttpUrl())
                        {
                            addonDirectory = Path.Combine(Paths.ExecutingDirectory, "PlayniteAddonDatabase");
                            FileSystem.DeleteDirectory(addonDirectory, true);

                            var info = new ProcessStartInfo("git")
                            {
                                Arguments = $"clone --depth=1 --branch=master {settings.Settings.Addons.AddonRepository}",
                                WorkingDirectory = Paths.ExecutingDirectory,
                                CreateNoWindow = true,
                                UseShellExecute = false,
                            };

                            using (var proc = Process.Start(info))
                                proc.WaitForExit();
                        }

                        foreach (var manifestFile in Directory.GetFiles(addonDirectory, "*.yaml", SearchOption.AllDirectories))
                        {
                            try
                            {
                                var manifest = Serialization.FromYamlFile<AddonManifestBase>(manifestFile);
                                if (manifest.AddonId.IsNullOrWhiteSpace())
                                {
                                    logger.Error($"Addon {manifestFile} doesn't have addon ID specified!");
                                    continue;
                                }

                                col.InsertOne(manifest);
                            }
                            catch (Exception e)
                            {
                                logger.Error(e, $"Failed to parse addon manifest {manifestFile}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, "Failed to generate addon database.");
                        throw;
                    }
                }
            });
        }
    }
}
