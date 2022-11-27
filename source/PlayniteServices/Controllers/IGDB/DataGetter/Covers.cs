﻿using Microsoft.AspNetCore.Mvc;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class Covers : DataGetter<GameImage>
    {
        public Covers(IgdbApi igdbApi) : base(igdbApi, "covers")
        {
        }
    }
}
