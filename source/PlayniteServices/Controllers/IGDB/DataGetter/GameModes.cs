﻿using Microsoft.AspNetCore.Mvc;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class GameModes : DataGetter<GameMode>
    {
        public GameModes(IgdbApi igdbApi) : base(igdbApi, "game_modes")
        {
        }
    }
}
