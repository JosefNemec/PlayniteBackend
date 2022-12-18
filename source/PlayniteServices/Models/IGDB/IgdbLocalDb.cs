using System;
using System.Collections.Generic;
using System.Text;

namespace PlayniteServices.Models.IGDB
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class SteamIdGame
    {
        public ulong steamId { get; set; }
        public ulong igdbId { get; set; }
    }

    public class GameIdMatch
    {
        public string Id { get; set; }
        public Guid Library { get; set; }
        public string GameId { get; set; }
        public ulong IgdbId { get; set; }
    }

    public class SearchIdMatch
    {
        public string Id { get; set; }
        public string Term { get; set; }
        public ulong IgdbId { get; set; }
    }

    public class IgdbSearchResult
    {
        public string Id { get; set; }
        public List<ulong> Games { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
