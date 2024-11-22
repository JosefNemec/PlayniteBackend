using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Playnite;
using System.IO;
using Playnite.Backend.GitHub;
using Playnite.Backend.Webhooks;
using IO = System.IO;

namespace Playnite.Backend.Addons;

public class AddonRequest
{
    public string? AddonId { get; set; }
    public string? SearchTerm { get; set; }
    public AddonType? Type { get; set; }
}

[Route("addons")]
public class AddonsController : Controller
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private readonly UpdatableAppSettings settings;
    private readonly Database db;
    private readonly AddonsManager addons;

    public AddonsController(UpdatableAppSettings settings, Database db, AddonsManager addons)
    {
        this.settings = settings;
        this.db = db;
        this.addons = addons;
    }

    [HttpGet("blacklist")]
    public DataResponse<string[]> GetBlackList()
    {
        return new DataResponse<string[]>(settings.Settings.Addons?.Blacklist ?? Array.Empty<string>());
    }

    [HttpGet("defaultextensions")]
    public DataResponse<string> GetDefaultExtensions()
    {
        if (settings.Settings.Addons?.DefaultExtensionsFile == null || settings.Settings.Addons?.DefaultExtensionsFile.IsNullOrWhiteSpace() == true)
        {
            return new DataResponse<string>(null);
        }

        var extensionFile = settings.Settings.Addons?.DefaultExtensionsFile!;
        if (!Path.IsPathRooted(extensionFile))
        {
            extensionFile = Path.Combine(PlaynitePaths.RuntimeDataDir, extensionFile);
        }

        if (IO.File.Exists(extensionFile))
        {
            return new DataResponse<string>(IO.File.ReadAllText(extensionFile));
        }

        return new DataResponse<string>(null);
    }

    [HttpGet]
    public async Task<DataResponse<List<AddonManifestBase>>> GetAddons([FromQuery]AddonRequest request)
    {
        var col = db.Addons;
        var result = new List<AddonManifestBase>();
        if (!request.AddonId.IsNullOrEmpty())
        {
            var filter = Builders<AddonManifestBase>.Filter.Eq(u => u.AddonId, request.AddonId);
            result = await col.Find(filter).ToListAsync();
        }
        else
        {
            // TODO convert to proper query
            foreach (var addon in await col.Find(Builders<AddonManifestBase>.Filter.Empty).ToListAsync())
            {
                if (request.Type != null && addon.Type != request.Type)
                {
                    continue;
                }

                if (!request.SearchTerm.IsNullOrEmpty())
                {
                    if (!(addon.Name?.Contains(request.SearchTerm, StringComparison.InvariantCultureIgnoreCase) == true ||
                          addon.Tags?.ContainsStringPartial(request.SearchTerm, StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        continue;
                    }
                }

                result.Add(addon);
            }
        }

        return new DataResponse<List<AddonManifestBase>>(result);
    }

    [ServiceFilter(typeof(ServiceKeyFilter))]
    [HttpPost("build")]
    public async Task<ActionResult> RegenerateDatabase()
    {
        await addons.RegenerateAddonDatabase();
        return Ok();
    }

    [HttpPost("githubhook")]
    public async Task<ActionResult> GithubWebhook()
    {
        if (settings.Settings.Addons?.GitHubSecret.IsNullOrWhiteSpace() == true)
        {
            logger.Error("Can't process addons github webhook, secret not configured.");
            return Ok();
        }

        if (Request.Headers.TryGetValue("X-Hub-Signature", out var sig))
        {
            if (!Request.Headers.TryGetValue("X-GitHub-Event", out var eventType))
            {
                logger.Error("X-GitHub-Event missing.");
                return BadRequest("No event.");
            }

            var payloadString = string.Empty;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                payloadString = await reader.ReadToEndAsync();
            }

            var payloadHash = GitHubWebHookController.GetPayloadHash(payloadString, settings.Settings.Addons!.GitHubSecret!);
            if (sig != $"sha1={payloadHash}")
            {
                logger.Error("PayloadHash signature check failed.");
                return BadRequest("Signature check failed.");
            }

            if (eventType == WebHookEvents.Push)
            {
                var payload = Serialization.FromJson<PushEvent>(payloadString);
                if (payload?.@ref?.EndsWith("master", StringComparison.Ordinal) == true)
                {
#pragma warning disable CS4014
                    addons.RegenerateAddonDatabase();
#pragma warning restore CS4014
                }
            }

            return Ok();
        }

        logger.Error("Addons github webhook not processed correctly.");
        return BadRequest();
    }
}
