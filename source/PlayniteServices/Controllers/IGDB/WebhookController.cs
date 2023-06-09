using Microsoft.AspNetCore.Mvc;
using Playnite;
using System.IO;

namespace PlayniteServices.IGDB;

public abstract class WebhookController<T> : Controller where T : class, IIgdbItem
{
    private static readonly ILogger logger = LogManager.GetLogger();

    private const string createAction = "create";
    private const string deleteAction = "delete";
    private const string updateAction = "update";

    private readonly UpdatableAppSettings settings;
    private readonly IgdbManager igdb;
    private readonly IgdbCollection<T> collection;
    public readonly string EndpointPath;

    public WebhookController(string endpointPath, IgdbManager igdb, UpdatableAppSettings settings)
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

    public class TempItem
    {
        public ulong id { get; set; }
    }

    private Task TryFetchUpdatedItem(ulong itemId)
    {
        return Task.Run(async () =>
        {
            await collection.Delete(itemId);
            var item = await collection.GetItem(itemId, true);
        });
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
            Exception? serError = null;
            T? item = null;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonString = await reader.ReadToEndAsync();
                if (!string.IsNullOrEmpty(jsonString))
                {
                    Serialization.TryFromJson(jsonString, out item, out serError);
                }
            }

            if (item == null)
            {
                // IGDB sometimes delives payload with missing propertie or numeric properties set to "null"
                // but the actual data in the DB are correct, so we fetch them manually.
                if (Serialization.TryFromJson<TempItem>(jsonString, out var tempItem, out _) &&
                    tempItem?.id > 0)
                {
                    logger.Warn($"Trying to fix broken {actionDescription} {EndpointPath} webhook from IGDB: {tempItem.id}");
#pragma warning disable CS4014
                    if (actionDescription == deleteAction)
                    {
                        collection.Delete(tempItem.id);
                    }
                    else
                    {
                        TryFetchUpdatedItem(tempItem.id);
                    }
#pragma warning restore CS4014

                    return Ok();
                }

                logger.Error($"Failed {actionDescription} {EndpointPath} webhook content deserialization.");
                if (serError != null)
                {
                    logger.Error(serError.Message);
                }

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

    [HttpPost(createAction)]
    public async Task<ActionResult> Create()
    {
        return await ProcessHook(collection.Add, createAction);
    }

    [HttpPost(deleteAction)]
    public async Task<ActionResult> Delete()
    {
        return await ProcessHook(collection.Delete, deleteAction);
    }

    [HttpPost(updateAction)]
    public async Task<ActionResult> Update()
    {
        return await ProcessHook(collection.Add, updateAction);
    }
}