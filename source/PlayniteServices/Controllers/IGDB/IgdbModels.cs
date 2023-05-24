using MongoDB.Bson.Serialization.Attributes;

namespace PlayniteServices.Controllers.IGDB;

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

public partial class Game
{
    [BsonIgnoreIfDefault] public double textScore { get; set; }
}

public partial class AlternativeName
{
    [BsonIgnoreIfDefault] public double textScore { get; set; }
}

public class TextSearchResult
{
    public double TextScore { get; set; }
    public string Name { get; set; }
    public Game Game { get; set; }

    public TextSearchResult(double textScore, string name, Game game)
    {
        TextScore = textScore;
        Name = name;
        Game = game;
    }
}