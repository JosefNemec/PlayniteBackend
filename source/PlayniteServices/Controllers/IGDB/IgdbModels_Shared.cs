// This needs to be separate from other models and C# 7 compatible
// because it's being consumed by IGDB plugin, which is still on .NET Framework.

using System;

#nullable enable
namespace PlayniteServices.Controllers.IGDB
{
    public interface IIgdbItem
    {
        ulong id { get; set; }
    }

    public class SearchRequest
    {
        public string? SearchTerm { get; set; }

        public SearchRequest()
        {
        }

        public SearchRequest(string searchTerm)
        {
            SearchTerm = searchTerm;
        }
    }

    public class MetadataRequest
    {
        public Guid LibraryId { get; set; }
        public string? GameId { get; set; }
        public string? Name { get; set; }
        public ulong PlatformId { get; set; }
        public int ReleaseYear { get; set; }

        public MetadataRequest()
        {
        }

        public MetadataRequest(string name)
        {
            Name = name;
        }
    }
}