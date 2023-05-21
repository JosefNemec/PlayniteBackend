using MongoDB.Bson.Serialization.Attributes;

namespace PlayniteServices.Controllers.IGDB;

public interface IIgdbItem
{
    ulong id { get; set; }
}

public class Webhook
{
    public int id { get; set; }
    public string? url { get; set; }
    public int category { get; set; }
    public int sub_category { get; set; }
    public bool active { get; set; }
    public int number_of_retries { get; set; }
    public string? api_key { get; set; }
    public string? secret { get; set; }
    public int created_at { get; set; }
    public int updated_at { get; set; }
}

public class SearchRequest
{
    public string? SearchTerm { get; set; }
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

//public partial class AlternativeName
//{
//    public double textScore { get; set; }
//}

//public partial class Game
//{
//    public double textScore { get; set; }
//}