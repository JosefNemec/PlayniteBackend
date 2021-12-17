using System;
using System.Collections.Generic;
using System.Text;

namespace PlayniteServices.Models.IGDB
{
    [Obsolete]
    public class SteamIdGame
    {
        public ulong steamId { get; set; }
        public ulong igdbId { get; set; }
    }

    [Obsolete]
    public class GameIdMatch
    {
        public string Id { get; set; }
        public Guid Library { get; set; }
        public string GameId { get; set; }
        public ulong IgdbId { get; set; }
    }

    [Obsolete]
    public class SearchIdMatch
    {
        public string Id { get; set; }
        public string Term { get; set; }
        public ulong IgdbId { get; set; }
    }

    [Obsolete]
    public class IgdbSearchResult
    {
        public string Id { get; set; }
        public List<ulong> Games { get; set; }
    }

    public class IgdbGameMatch
    {
        public string MatchId { get; set; }
        public ulong GameId { get; set; }
    }
}
