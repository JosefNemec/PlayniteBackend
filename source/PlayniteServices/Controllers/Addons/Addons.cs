using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Controllers.Webhooks;
using PlayniteServices.Filters;
using PlayniteServices.Models.GitHub;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.Addons
{
    public class AddonRequest
    {
        public string AddonId { get; set; }
        public string SearchTerm { get; set; }
        public AddonType? Type { get; set; }
    }

    [Route("addons")]
    public class AddonsController : Controller
    {
        private readonly static object generatorLock = new object();
        private readonly static ILogger logger = LogManager.GetLogger();
        private UpdatableAppSettings settings;

        public AddonsController(UpdatableAppSettings settings)
        {
            this.settings = settings;
        }

        [HttpGet()]
        public ServicesResponse<List<AddonManifestBase>> GetAddons([FromQuery]AddonRequest request)
        {
            var col = Program.AddonsCollection;
            List<AddonManifestBase> addons = null;
            if (!request.AddonId.IsNullOrEmpty())
            {
                addons = col.Find(a => a.AddonId == request.AddonId).ToList();
            }
            else
            {
                var query = col.FindAll();
                if (!request.SearchTerm.IsNullOrEmpty())
                {
                    query = query.Where(a =>
                        a.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        a.Tags.ContainsString(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (request.Type != null)
                {
                    query = query.Where(a => a.Type == request.Type);
                }

                addons = query.ToList();
            }

            return new ServicesResponse<List<AddonManifestBase>>(addons);
        }

        [ServiceFilter(typeof(ServiceKeyFilter))]
        [HttpPost("build")]
        public async Task<ActionResult> RegenerateDatabase()
        {
            await RegenerateAddonDatabase();
            return Ok();
        }

        [HttpPost("githubhook")]
        public async Task<ActionResult> GithubWebhook()
        {
            if (Request.Headers.TryGetValue("X-Hub-Signature", out var sig))
            {
                if (!Request.Headers.TryGetValue("X-GitHub-Event", out var eventType))
                {
                    logger.Error("X-GitHub-Event missing.");
                    return BadRequest("No event.");
                }

                string payloadString = null;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    payloadString = await reader.ReadToEndAsync();
                }

                var payloadHash = GitHubController.GetPayloadHash(payloadString, settings.Settings.Addons.GitHubSecret);
                if (sig != $"sha1={payloadHash}")
                {
                    logger.Error("PayloadHash signature check failed.");
                    return BadRequest("Signature check failed.");
                }

                if (eventType == WebHookEvents.Push)
                {
                    var payload = Serialization.FromJson<PushEvent>(payloadString);
                    if (payload.@ref?.EndsWith("master") == true)
                    {
#pragma warning disable CS4014
                        RegenerateAddonDatabase();
#pragma warning restore CS4014
                    }
                }

                return Ok();
            }

            logger.Error("Addons github webhook not processed correctly.");
            return BadRequest();
        }

        public Task RegenerateAddonDatabase()
        {
            return Task.Factory.StartNew(() =>
            {
                logger.Info("Regenerating addons database.");
                lock (generatorLock)
                {
                    var repoPackage = Path.Combine(Paths.ExecutingDirectory, "repo.zip");
                    FileSystem.DeleteFile(repoPackage);
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadFile(settings.Settings.Addons.AddonRepository, repoPackage);
                    }

                    var col = Program.AddonsCollection;
                    col.Delete(Query.All());

                    using (var zip = ZipFile.OpenRead(repoPackage))
                    {
                        foreach (var manifestFile in zip.Entries.Where(a => a.Name.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)))
                        {
                            using (var entry = zip.GetEntry(manifestFile.FullName).Open())
                            {
                                try
                                {
                                    var manifest = Serialization.FromYamlStream<AddonManifestBase>(entry);
                                    col.Upsert(manifest);
                                }
                                catch (Exception e)
                                {
                                    logger.Error(e, $"Failed to parse addon manifest {manifestFile.FullName}");
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
