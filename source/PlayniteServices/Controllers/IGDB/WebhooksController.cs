using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Playnite.SDK;
using Playnite.SDK.Data;
using PlayniteServices.Controllers.IGDB.DataGetter;
using PlayniteServices.Databases;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB
{
    [Route("igdb/webhooks")]
    public class WebhooksController : Controller
    {
        enum IgdbWebhookMethod
        {
            Create,
            Update,
            Delete
        }

        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly UpdatableAppSettings settings;
        private readonly IgdbApi igdbApi;

        public WebhooksController(UpdatableAppSettings settings, IgdbApi igdbApi)
        {
            this.settings = settings;
            this.igdbApi = igdbApi;
        }

        private async Task<ActionResult> ProcessWebhook<T>(IgdbWebhookMethod method, DataGetter<T> getter) where T : IgdbItem
        {
            if (Request.Headers.TryGetValue("X-Secret", out var secret))
            {
                if (secret != settings.Settings.IGDB.WebHookSecret)
                {
                    logger.Error($"X-Secret doesn't match: {secret}");
                    return BadRequest();
                }

                try
                {
                    T webhookItem = null;
                    string jsonString = null;
                    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        jsonString = await reader.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            webhookItem = Serialization.FromJson<T>(jsonString);
                        }
                    }

                    if (webhookItem == null)
                    {
                        logger.Error($"Failed IGDB webhook content serialization: {getter.EndpointPath} {method}.");
                        return Ok();
                    }

                    logger.Info($"Received {getter.EndpointPath} {method} webhook from IGDB: {webhookItem.id}");
                    switch (method)
                    {
                        case IgdbWebhookMethod.Create:
                        case IgdbWebhookMethod.Update:
                            getter.Collection.ReplaceOne(
                                Builders<T>.Filter.Eq(a => a.id, webhookItem.id),
                                webhookItem,
                                Database.ItemUpsertOptions);
                            break;
                        case IgdbWebhookMethod.Delete:
                            getter.Collection.DeleteOne(
                                Builders<T>.Filter.Eq(a => a.id, webhookItem.id));
                            break;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to process {getter.EndpointPath} {method} IGDB webhook.");
                }

                return Ok();
            }
            else
            {
                logger.Error($"Missing X-Secret from {getter.EndpointPath} {method} IGDB webhook.");
                return BadRequest();
            }
        }

        [HttpPost("games/create")]
        public async Task<ActionResult> GameCreate()
        {
            return await ProcessWebhook(IgdbWebhookMethod.Create, igdbApi.Games);
            // download childs
        }

        [HttpPost("games/delete")]
        public async Task<ActionResult> GameDelete()
        {
            return await ProcessWebhook(IgdbWebhookMethod.Delete, igdbApi.Games);
            // delete any search matches
            // delete external game references
        }

        [HttpPost("games/update")]
        public async Task<ActionResult> GameUpdate()
        {
            return await ProcessWebhook(IgdbWebhookMethod.Update, igdbApi.Games);
            // download childs
        }
    }
}
