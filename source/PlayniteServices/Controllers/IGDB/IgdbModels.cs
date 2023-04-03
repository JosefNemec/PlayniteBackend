namespace PlayniteServices.Controllers.IGDB;

public class IgdbItem
{
    public ulong id { get; set; }
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

//public static class ModelsUtils
//{
//    public static string GetIgdbSearchString(string gameName)
//    {
//        var temp =
//            gameName.Replace(":", " ", StringComparison.InvariantCultureIgnoreCase).
//            ToLower().
//            Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase).
//            Trim();
//        return Regex.Replace(temp, @"\s+", " ");
//    }
//}