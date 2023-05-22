using Microsoft.AspNetCore.Mvc;
using Playnite.SDK;
using System.IO;

namespace PlayniteServices.Controllers.IGDB;

public abstract class WebhookController<T> : Controller where T : class, IIgdbItem
{
    private static readonly ILogger logger = LogManager.GetLogger();

    private readonly UpdatableAppSettings settings;
    private readonly IgdbApi igdb;
    private readonly IgdbCollection<T> collection;
    public readonly string EndpointPath;

    public WebhookController(string endpointPath, IgdbApi igdb, UpdatableAppSettings settings)
    {
        EndpointPath = endpointPath;
        this.igdb = igdb;
        this.settings = settings;
        collection = (IgdbCollection<T>)igdb.DataCollections.First(a => a.EndpointPath == endpointPath);
    }

    private bool ValidateWebhook()
    {
        if (settings.Settings.IGDB?.WebHookSecret.IsNullOrWhiteSpace() == true)
        {
            logger.Error("Can't process IGDB webhook, webhook secret is not configured");
            return false;
        }

        if (Request.Headers.TryGetValue("X-Secret", out var secret))
        {
            if (secret != settings.Settings.IGDB!.WebHookSecret!)
            {
                logger.Error($"X-Secret doesn't match: {secret}");
                return false;
            }

            return true;
        }
        else
        {
            logger.Error("Missing X-Secret from IGDB webhook.");
            return false;
        }
    }

    private async Task<ActionResult> ProcessHook(Func<T, Task> itemAction, string actionDescription)
    {
        if (!ValidateWebhook())
        {
            return BadRequest();
        }

        var jsonString = string.Empty;
        try
        {
            T? item = null;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonString = await reader.ReadToEndAsync();
                if (!string.IsNullOrEmpty(jsonString))
                {
                    item = DataSerialization.FromJson<T>(jsonString);
                }
            }

            if (item == null)
            {
                logger.Error($"Failed {actionDescription} {EndpointPath} webhook content deserialization.");
                logger.Debug(jsonString);
                return Ok();
            }

            logger.Info($"Received {actionDescription} {EndpointPath} webhook from IGDB: {item.id}");
            await itemAction(item);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to process {actionDescription} {EndpointPath} webhook.");
            logger.Debug(jsonString);
        }

        return Ok();
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create()
    {
        return await ProcessHook(collection.Add, "create");
    }

    [HttpPost("delete")]
    public async Task<ActionResult> Delete()
    {
        return await ProcessHook(collection.Delete, "delete");
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update()
    {
        return await ProcessHook(collection.Add, "update");
    }
}