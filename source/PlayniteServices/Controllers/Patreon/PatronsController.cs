using JsonApiSerializer;
using JsonApiSerializer.JsonApi;
using Microsoft.AspNetCore.Mvc;
using Playnite.SDK;
using PlayniteServices.Filters;
using PlayniteServices.Models.Patreon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.Patreon
{
    [ServiceFilter(typeof(PlayniteVersionFilter))]
    [Route("patreon/patrons")]
    public class PatronsController : Controller
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly PlayniteServices.Patreon patreon;

        public class ListCache
        {
            public DateTime Created;
            public List<string> Names;

            public ListCache(List<string> names)
            {
                Created = DateTime.Now;
                Names = names;
            }
        }

        private static ListCache? cache;

        public PatronsController(PlayniteServices.Patreon patreon)
        {
            this.patreon = patreon;
        }

        [HttpGet]
        public async Task<DataResponse<List<string>>> Get()
        {
            if (cache == null || (DateTime.Now - cache.Created).TotalSeconds > 60)
            {
                var allPatrons = new List<string>();
                string nextLink = "api/campaigns/1400397/pledges?include=patron.null&page%5Bcount%5D=9999";

                do
                {
                    var stringData = await patreon.SendStringRequest(nextLink);
                    var document = Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentRoot<Pledge[]>>(stringData, new JsonApiSerializerSettings());
                    if (document == null)
                    {
                        logger.Error("Failed to get list of patrons, no data from API.");
                        return new DataResponse<List<string>>(new List<string>());
                    }

                    if (document.Errors.HasItems())
                    {
                        logger.Error("Failed to get list of patrons.");
                        document.Errors.ForEach(a => logger.Error(a.Detail));
                        return new DataResponse<List<string>>(new List<string>());
                    }

                    allPatrons.AddRange(document.Data.Where(a => a.declined_since == null).Select(a => a.patron?.full_name ?? string.Empty));
                    if (document.Links.TryGetValue("next", out var value))
                    {
                        nextLink = value.Href;
                    }
                    else
                    {
                        break;
                    }
                }
                while (!nextLink.IsNullOrEmpty());

                cache = new ListCache(allPatrons.OrderBy(a => a).ToList());
            }

            return new DataResponse<List<string>>(cache.Names);
        }
    }
}
