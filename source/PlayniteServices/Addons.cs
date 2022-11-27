using MongoDB.Bson;
using MongoDB.Driver;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class Addons : IDisposable
    {
        private static bool instantiated = false;
        private readonly static object generatorLock = new object();
        private readonly static ILogger logger = LogManager.GetLogger();
        private readonly UpdatableAppSettings settings;
        private readonly Database db;
        private readonly HttpClient httpClient;
        private readonly System.Threading.Timer addonUpdatesTimer;

        public event EventHandler InstallerManifestsUpdated;

        public Addons(UpdatableAppSettings settings, Database db)
        {
            TestAssert.IsFalse(instantiated, $"{nameof(Addons)} already instantiated");
            instantiated = true;
            this.settings = settings;
            this.db = db;
            httpClient = new HttpClient { Timeout = new TimeSpan(0, 0, 20) };

            if (settings.Settings.Addons.AutoUpdate)
            {
                addonUpdatesTimer = new System.Threading.Timer(
                    async (_) => await UpdateAddonInstallers(),
                    null,
                    new TimeSpan(0),
                    new TimeSpan(0, 15, 0));
            }
        }

        public void Dispose()
        {
            addonUpdatesTimer?.Dispose();
            httpClient.Dispose();
        }

        private async Task UpdateAddonInstallers()
        {
            logger.Info("Updating addon installers cache.");
            var anyUpdates = false;
            try
            {
                foreach (var addon in db.Addons.AsQueryable())
                {
                    var newInstaller = await GetInstallerManifest(addon);
                    if (newInstaller?.Packages.HasItems() == true)
                    {
                        var newData = false;
                        var existing = db.AddonInstallers.AsQueryable().FirstOrDefault(a => a.AddonId == addon.AddonId);
                        if (existing != null)
                        {
                            if (existing.Packages.Max(a => a.Version) != newInstaller.Packages.Max(a => a.Version))
                            {
                                newData = true;
                            }
                        }
                        else
                        {
                            newData = true;
                        }

                        if (newData)
                        {
                            anyUpdates = true;
                            db.AddonInstallers.ReplaceOne(
                                a => a.AddonId == newInstaller.AddonId,
                                newInstaller,
                                Database.ItemUpsertOptions);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to update addon installer cache.");
                return;
            }

            if (anyUpdates)
            {
                InstallerManifestsUpdated?.Invoke(this, null);
            }
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
                return DataSerialization.FromYaml<AddonInstallerManifestBase>(manifestStr);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to download addon {addon.AddonId} installer manifest {addon.InstallerManifestUrl}");
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
                        var col = db.Addons;
                        col.DeleteMany(new BsonDocument());

                        var addonDirectory = settings.Settings.Addons.AddonRepository;
                        if (settings.Settings.Addons.AddonRepository.IsHttpUrl())
                        {
                            addonDirectory = Path.Combine(ServicePaths.ExecutingDirectory, "PlayniteAddonDatabase");
                            FileSystem.DeleteDirectory(addonDirectory, true);

                            var info = new ProcessStartInfo("git")
                            {
                                Arguments = $"clone --depth=1 --branch=master {settings.Settings.Addons.AddonRepository}",
                                WorkingDirectory = ServicePaths.ExecutingDirectory,
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
                                var manifest = DataSerialization.FromYamlFile<AddonManifestBase>(manifestFile);
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

                        logger.Info("Finished regenerating addons database.");
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
