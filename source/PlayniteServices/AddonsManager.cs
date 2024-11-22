using MongoDB.Bson;
using MongoDB.Driver;
using Playnite;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Playnite.Backend.Addons;

public class AddonsManager : IDisposable
{
    private static bool instantiated = false;
    private readonly static Lock generatorLock = new Lock();
    private readonly static ILogger logger = LogManager.GetLogger();
    private readonly UpdatableAppSettings settings;
    private readonly Database db;
    private readonly HttpClient httpClient;
    private Timer? addonUpdatesTimer;

    public event EventHandler? InstallerManifestsUpdated;

    public AddonsManager(UpdatableAppSettings settings, Database db)
    {
        if (settings.Settings.Addons == null)
        {
            throw new Exception("Addon settings are missing.");
        }

        TestAssert.IsFalse(instantiated, $"{nameof(AddonsManager)} already instantiated");
        instantiated = true;
        this.settings = settings;
        this.db = db;
        httpClient = new HttpClient { Timeout = new TimeSpan(0, 0, 20) };
    }

    public void Dispose()
    {
        addonUpdatesTimer?.Dispose();
        httpClient.Dispose();
    }

    public void StartUpdateChecker()
    {
        if (addonUpdatesTimer != null)
        {
            throw new Exception("Addon update checker already started.");
        }

        addonUpdatesTimer = new Timer(
            UpdateAddonInstallersCallback,
            null,
            new TimeSpan(0),
            new TimeSpan(0, 20, 0));
    }

    private async void UpdateAddonInstallersCallback(object? _)
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
                        if (!existing.Packages.HasItems())
                        {
                            newData = true;
                        }
                        else if (existing.Packages.Max(a => a.Version) != newInstaller.Packages.Max(a => a.Version))
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
                        await db.AddonInstallers.ReplaceOneAsync(
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
            InstallerManifestsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task<AddonInstallerManifestBase?> GetInstallerManifest(AddonManifestBase addon)
    {
        try
        {
            var manifestStr = await httpClient.GetStringAsync(addon.InstallerManifestUrl);
            return Serialization.FromYaml<AddonInstallerManifestBase>(manifestStr);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to download addon {addon.AddonId} installer manifest {addon.InstallerManifestUrl}");
            return null;
        }
    }

    public Task RegenerateAddonDatabase()
    {
        return Task.Run(() =>
        {
            if (settings.Settings.Addons!.AddonRepository.IsNullOrEmpty())
            {
                logger.Error("Addons repository path is not specified.");
                return;
            }

            logger.Info("Regenerating addons database.");
            lock (generatorLock)
            {
                try
                {
                    var col = db.Addons;
                    col.DeleteMany(new BsonDocument());

                    var addonDirectory = settings.Settings.Addons!.AddonRepository;
                    if (settings.Settings.Addons!.AddonRepository.IsHttpUrl())
                    {
                        addonDirectory = Path.Combine(PlaynitePaths.RuntimeDataDir, "PlayniteAddonDatabase");
                        FileSystem.DeleteDirectory(addonDirectory, true);

                        var info = new ProcessStartInfo("git")
                        {
                            Arguments = $"clone --depth=1 --branch=master {settings.Settings.Addons.AddonRepository}",
                            WorkingDirectory = PlaynitePaths.RuntimeDataDir,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                        };

                        using var proc = Process.Start(info);
                        proc!.WaitForExit();
                        if (proc.ExitCode != 0)
                        {
                            throw new Exception($"Addon repo clone failed: {proc.ExitCode}");
                        }
                    }

                    if (!Directory.Exists(addonDirectory))
                    {
                        throw new Exception("Addon repo directory not found.");
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
