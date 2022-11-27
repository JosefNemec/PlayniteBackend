using ComposableAsync;
using MongoDB.Driver;
using Playnite;
using Playnite.SDK;
using PlayniteServices.Models.Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class Discord : IDisposable
    {
        private static bool instantiated = false;
        private readonly static ILogger logger = LogManager.GetLogger();
        private const string apiBaseUrl = @"https://discord.com/api/v9/";
        private readonly UpdatableAppSettings settings;
        private readonly Addons addons;
        private readonly Database db;
        private readonly HttpClient httpClient;
        private readonly ConcurrentDictionary<string, ConcurrentQueue<HttpRequestMessage>> messageQueues = new ConcurrentDictionary<string, ConcurrentQueue<HttpRequestMessage>>();
        private readonly ConcurrentDictionary<string, RateLimitHeaders> rateLimits = new ConcurrentDictionary<string, RateLimitHeaders>();

        private string addonsFeedChannel;

        public Discord(UpdatableAppSettings settings, Addons addons, Database db)
        {
            TestAssert.IsFalse(instantiated, $"{nameof(Discord)} already instantiated");
            instantiated = true;
            this.settings = settings;
            this.addons = addons;
            this.db = db;
            if (!settings.Settings.Discord.BotEnabled)
            {
                return;
            }

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {settings.Settings.Discord.BotToken}");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PlayniteBot/1.0");
            httpClient.Timeout = new TimeSpan(0, 0, 20);
            var initRes = Init().GetAwaiter().GetResult();
            if (initRes)
            {
                logger.Info("Discord bot enabled.");
                addons.InstallerManifestsUpdated += Addons_InstallerManifestsUpdated;
            }
        }

        public async Task<bool> Init()
        {
            try
            {
                var guilds = await Get<List<Guild>>(@"users/@me/guilds");
                var channels = await Get<List<Channel>>($"guilds/{guilds[0].id}/channels");
                addonsFeedChannel = channels.First(a => a.name == "addons-feed").id;
                await ProcessAddonUpdates(true);
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to init Discord bot.");
                return false;
            }
        }

        public void Dispose()
        {
            addons.InstallerManifestsUpdated -= Addons_InstallerManifestsUpdated;
            httpClient?.Dispose();
        }

        private static string AddonTypeToFriendlyString(AddonType type)
        {
            switch (type)
            {
                case AddonType.GameLibrary:
                    return "library plugin";
                case AddonType.MetadataProvider:
                    return "metadata plugin";
                case AddonType.Generic:
                    return "extension";
                case AddonType.ThemeDesktop:
                    return "desktop theme";
                case AddonType.ThemeFullscreen:
                    return "fullscreen theme";
                default:
                    return "uknown";
            }
        }

        private async Task<Message> SendNewAddonNotif(AddonManifestBase addon)
        {
            var embed = new EmbedObject
            {
                author = new EmbedAuthor { name = addon.Author },
                description = addon.Description,
                thumbnail = addon.IconUrl.IsNullOrEmpty() ? null : new EmbedImage { url = addon.IconUrl },
                title = $"{addon.Name} {AddonTypeToFriendlyString(addon.Type)} has been released",
                url = "https://playnite.link/addons.html#{0}".Format(Uri.EscapeDataString(addon.AddonId)),
                image = addon.Screenshots.HasItems() ? new EmbedImage { url = addon.Screenshots[0].Image } : null,
                color = 0x19d900
            };

            return await SendMessage(addonsFeedChannel, string.Empty, new List<EmbedObject> { embed });
        }

        private async Task<Message> SendAddonUpdateNotif(AddonManifestBase addon, AddonInstallerPackage package)
        {
            var description = $"Updated to {package.Version}:\n\n";
            if (package.Changelog.HasItems())
            {
                description += string.Join("\n", package.Changelog.Select(a => $"- {a}"));
            }
            else
            {
                description += "Changelog missing!";
            }

            var embed = new EmbedObject
            {
                author = new EmbedAuthor { name = addon.Author },
                description = description,
                thumbnail = addon.IconUrl.IsNullOrEmpty() ? null : new EmbedImage { url = addon.IconUrl },
                url = "https://playnite.link/addons.html#{0}".Format(Uri.EscapeDataString(addon.AddonId)),
                title = addon.Name,
                color = 0xbf0086
            };

            return await SendMessage(addonsFeedChannel, string.Empty, new List<EmbedObject> { embed });
        }

        private async Task ProcessAddonUpdates(bool sendNotifications)
        {
            foreach (var addon in db.Addons.AsQueryable())
            {
                var installer = db.AddonInstallers.AsQueryable().FirstOrDefault(a => a.AddonId == addon.AddonId);
                if (installer == null)
                {
                    continue;
                }

                var latestPackage = installer.Packages?.OrderByDescending(a => a.Version).FirstOrDefault();
                if (latestPackage == null)
                {
                    logger.Error($"Addon {addon.AddonId} has no install packages.");
                    continue;
                }

                Message sentMessage = null;
                var lastNotif = db.DiscordAddonNotifications.AsQueryable().FirstOrDefault(a => a.AddonId == addon.AddonId);
                if (lastNotif == null)
                {
                    if (sendNotifications)
                    {
                        sentMessage = await SendNewAddonNotif(addon);
                    }
                }
                else
                {
                    if (lastNotif.NotifyVersion == latestPackage.Version)
                    {
                        continue;
                    }

                    if (sendNotifications)
                    {
                        sentMessage = await SendAddonUpdateNotif(addon, latestPackage);
                    }
                }

                // Only publish new addon releases because of crappy Discord notification limit
                if (sentMessage != null && lastNotif == null)
                {
                    await Post<Message>($"channels/{addonsFeedChannel}/messages/{sentMessage.id}/crosspost");
                }

                db.DiscordAddonNotifications.ReplaceOne(
                    a => a.AddonId == addon.AddonId,
                    new AddonUpdateNotification
                    {
                        AddonId = addon.AddonId,
                        Date = DateTimeOffset.Now,
                        NotifyVersion = latestPackage.Version
                    },
                    Database.ItemUpsertOptions);
            }
        }

        private async void Addons_InstallerManifestsUpdated(object sender, EventArgs e)
        {
            if (addonsFeedChannel.IsNullOrEmpty())
            {
                return;
            }

            logger.Info("Processing Discord addon notifications.");
            try
            {
                await ProcessAddonUpdates(true);
            }
            catch (Exception exc)
            {
                logger.Error(exc, "Failed to process addon updates.");
            }
        }

        private async Task<Message> SendMessage(string channelId, string message, List<EmbedObject> embeds = null)
        {
            return await PostJson<Message>($"channels/{channelId}/messages", new MessageCreate
            {
                content = message,
                embeds = embeds
            });
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage message) where T : class
        {
            var route = message.RequestUri.OriginalString.Substring(apiBaseUrl.Length);
            route = route.Substring(0, route.IndexOf('/', StringComparison.Ordinal));

            var messageQueue = messageQueues.GetOrAdd(route, new ConcurrentQueue<HttpRequestMessage>());
            messageQueue.Enqueue(message);
            while (messageQueue.TryPeek(out var currentMessage) && currentMessage != message)
            {
                await Task.Delay(100);
            }

            var routeLimits = rateLimits.GetOrAdd(route, new RateLimitHeaders());
            if (routeLimits.Remaining <= 0)
            {
                logger.Warn($"Exhausted Discord rate limit on '{route}', waiting {routeLimits.ResetAfter}");
                await Task.Delay(routeLimits.ResetAfter);
            }

            var resp = await ClientSendMessage(message);
            var cnt = await resp.Content.ReadAsStringAsync();

            var limitHeaders = new RateLimitHeaders(resp.Headers);
            rateLimits.AddOrUpdate(route, limitHeaders, (_, __) => limitHeaders);
            messageQueue.TryDequeue(out var _);

            if (resp.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var limitResponse = DataSerialization.FromJson<RateLimitResponse>(cnt);
                logger.Warn($"Discord rate limit on '{route}' route, {limitResponse.global}, {limitResponse.retry_after}");
                await Task.Delay(TimeSpan.FromSeconds(limitResponse.retry_after + 0.1));
                return await SendRequest<T>(message);
            }
            else if (resp.StatusCode != HttpStatusCode.OK)
            {
                logger.Error(cnt);
                var error = DataSerialization.FromJson<Error>(cnt);
                throw new Exception($"Discord: {error.code}, {error.message}");
            }
            else
            {
                return DataSerialization.FromJson<T>(cnt);
            }
        }

        private async Task<HttpResponseMessage> ClientSendMessage(HttpRequestMessage message)
        {
            // Clone message, for case we are re-sending failed message
            // httpclient can't send already sent messages again.
            var request = new HttpRequestMessage(message.Method, message.RequestUri);
            if (message.Content != null)
            {
                request.Content = message.Content;
            }

            return await httpClient.SendAsync(request);
        }

        private async Task<T> Get<T>(string url) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiBaseUrl + url);
            return await SendRequest<T>(request);
        }

        private async Task<T> PostJson<T>(string url, object content) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiBaseUrl + url)
            {
                Content = new StringContent(DataSerialization.ToJson(content), Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            return await SendRequest<T>(request);
        }

        private async Task<T> Post<T>(string url) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiBaseUrl + url);
            return await SendRequest<T>(request);
        }
    }
}
