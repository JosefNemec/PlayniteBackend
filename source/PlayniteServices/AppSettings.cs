using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class SteamSettings
    {
        public string? ApiKey { get; set; }
    }

    public class IgdbSettings
    {
        public string? ApiEndpoint { get; set; }
        public string? ClientId { get; set; }
        public string? AccessToken { get; set; }
        public string? ClientSecret { get; set; }
        public string? WebHookSecret { get; set; }
        public string? WebHookRootAddress { get; set; }
        public bool RegisterWebhooks { get; set; }
    }

    public class PatreonSettings
    {
        public string? ApiEndpoint { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Secret { get; set; }
        public string? Id { get; set; }
    }

    public class GitHubSettings
    {
        public string? DiscordWebhookUrl { get; set; }
        public string? GitHubSecret { get; set; }
    }

    public class AddonsSettings
    {
        public string? DefaultExtensionsFile { get; set; }
        public string? AddonRepository { get; set; }
        public string? GitHubSecret { get; set; }
        public string[]? Blacklist { get; set; }
        public bool AutoUpdate { get; set; }
    }

    public class DiscordSettings
    {
        public string? BotToken { get; set; }
        public bool BotEnabled { get; set; }
    }

    public class AppSettings
    {
        public string? DatabaseConString { get; set; }
        public string? DiagsDirectory { get; set; }
        public string? ServiceKey { get; set; }
        public bool RestrictPlayniteVersion { get; set; }
        public List<string>? RestrictedPlayniteVersions { get; set; }
        public SteamSettings? Steam { get; set; }
        public IgdbSettings? IGDB { get; set; }
        public PatreonSettings? Patreon { get; set; }
        public GitHubSettings? GitHub { get; set; }
        public AddonsSettings? Addons { get; set; }
        public DiscordSettings? Discord { get; set; }
        public Version? MinimumDiagVersion { get; set; }
    }

    public class UpdatableAppSettings
    {
        public AppSettings Settings { get; private set; }

        public UpdatableAppSettings(IOptionsMonitor<AppSettings> settings)
        {
            Settings = settings.CurrentValue;
            settings.OnChange((s) => Settings = s);
        }
    }
}
