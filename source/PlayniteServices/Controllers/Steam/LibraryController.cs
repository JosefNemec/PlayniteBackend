﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading;
using PlayniteServices.Models.Steam;
using PlayniteServices.Filters;
using PlayniteServices.Databases;

namespace PlayniteServices.Controllers.Steam
{
    [ServiceFilter(typeof(PlayniteVersionFilter))]
    [Route("steam/library")]
    public class LibraryController : Controller
    {
        private static HttpClient httpClient = new HttpClient();
        private static double requestDelay = 1500;
        private static DateTime lastRequest = DateTime.Now.AddMilliseconds(-requestDelay);
        private static object dateLock = new object();

        // Steam API has limit one request per second, so we need to slow requests down
        // TODO: change this to something more sophisticated like proper queue
        private void WaitRequest()
        {
            lock (dateLock)
            {
                var timeDiff = DateTime.Now - lastRequest;

                if (timeDiff.TotalMilliseconds > requestDelay)
                {
                    lastRequest = DateTime.Now;
                    return;
                }

                Thread.Sleep((int)requestDelay - (DateTime.Now - lastRequest).Milliseconds);
                lastRequest = DateTime.Now;
            }
        }

        [HttpGet("{steamId}")]
        public async Task<ServicesResponse<List<GetOwnedGamesResult.Game>>> Get(ulong steamId, [FromQuery]bool freeSub)
        {
            var libraryUrl = string.Format(
                @"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&include_appinfo=1&format=json&steamid={1}&include_played_free_games=1&skip_unvetted_apps=0",
                Steam.ApiKey, steamId);
            if (freeSub)
            {
                libraryUrl += "&include_free_sub=1";
            }

            WaitRequest();

            var libraryStringResult = await httpClient.GetStringAsync(libraryUrl);
            var libraryResult = JsonConvert.DeserializeObject<GetOwnedGamesResult>(libraryStringResult);

            return new ServicesResponse<List<GetOwnedGamesResult.Game>>(libraryResult.response.games);
        }
    }
}
