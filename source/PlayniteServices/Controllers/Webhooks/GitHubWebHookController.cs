using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Playnite.SDK;
using PlayniteServices.Models.GitHub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.Webhooks
{
    [Route("hooks/github")]
    public class GitHubWebHookController : Controller
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly UpdatableAppSettings settings;

        public GitHubWebHookController(UpdatableAppSettings settings)
        {
            this.settings = settings;
        }

        public static string GetPayloadHash(string payload, string key)
        {
            var encoding = new UTF8Encoding();
            var textBytes = encoding.GetBytes(payload);
            var keyBytes = encoding.GetBytes(key);
            using (var hash = new HMACSHA1(keyBytes))
            {
                return BitConverter.ToString(hash.ComputeHash(textBytes)).Replace("-", "", StringComparison.Ordinal).ToLower();
            }
        }

        [HttpPost]
        public async Task<ActionResult> GithubWebhook()
        {
            if (settings.Settings.GitHub?.GitHubSecret.IsNullOrEmpty() == true)
            {
                logger.Error("Can't process github webhook, secret not configured.");
                return Ok();
            }

            if (Request.Headers.TryGetValue("X-Hub-Signature", out var sig))
            {
                if (!Request.Headers.TryGetValue("X-GitHub-Event", out var eventType))
                {
                    return BadRequest("No event.");
                }

                string payloadString = string.Empty;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    payloadString = await reader.ReadToEndAsync();
                }

                var payloadHash = GetPayloadHash(payloadString, settings.Settings.GitHub!.GitHubSecret!);
                if (sig != $"sha1={payloadHash}")
                {
                    return BadRequest("Signature check failed.");
                }

                var forwardEvent = true;
                if (eventType == WebHookEvents.Issues)
                {
                    var payload = DataSerialization.FromJson<IssuesEvent>(payloadString);
                    if (payload == null)
                    {
                        logger.Error("Unregognized github webhook Issues payload.");
                        logger.Debug(payloadString);
                        return Ok();
                    }

                    // Only forward opened issues
                    if (payload.action != IssuesEventAction.opened)
                    {
                        forwardEvent = false;
                        logger.Debug("Ignored non-opened github issue webhook.");
                    }
                }
                else if (eventType == WebHookEvents.Push)
                {
                    var payload = DataSerialization.FromJson<PushEvent>(payloadString);
                    if (payload == null)
                    {
                        logger.Error("Unregognized github webhook Push payload.");
                        logger.Debug(payloadString);
                        return Ok();
                    }

                    // Ignore localization pushes
                    if (payload.@ref?.EndsWith("l10n_devel", StringComparison.Ordinal) == true)
                    {
                        forwardEvent = false;
                        logger.Debug("Ignored l10n_devel github webhook.");
                    }
                    // Don't forward branch merges
                    else if (payload.commits?.Any(a => a.message?.StartsWith("Merge branch", StringComparison.OrdinalIgnoreCase) == true) == true)
                    {
                        forwardEvent = false;
                        payload.commits = payload.commits.Where(a => !a.message?.StartsWith("Merge branch", StringComparison.OrdinalIgnoreCase) == true).ToList();
                        if (payload.commits.HasItems())
                        {
                            logger.Debug("Forwarded commits without merge commits.");
                            await ForwardRequest(DataSerialization.ToJson(payload));
                        }
                        else
                        {
                            logger.Debug("Ignored commits with only merge commits.");
                        }
                    }
                }

                if (forwardEvent)
                {
                    await ForwardRequest(payloadString);
                }

                return Ok();
            }

            return BadRequest();
        }

        private async Task ForwardRequest(string payload)
        {
            if (settings.Settings.GitHub?.DiscordWebhookUrl.IsNullOrEmpty() == true)
            {
                logger.Error("Can't forward github webhook to discord, dicord url not configured.");
                return;
            }

            var cnt = new StringContent(payload, Encoding.UTF8, "application/json");
            cnt.Headers.Add("X-GitHub-Delivery", Request.Headers["X-GitHub-Delivery"].FirstOrDefault());
            cnt.Headers.Add("X-GitHub-Event", Request.Headers["X-GitHub-Event"].FirstOrDefault());
            var discordResp = await Program.HttpClient.PostAsync(
                settings.Settings.GitHub!.DiscordWebhookUrl,
                cnt);
            await discordResp.Content.ReadAsStringAsync();
        }
    }
}
