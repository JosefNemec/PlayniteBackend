namespace PlayniteServices.Utilities;

public partial class Program
{
    public static void Main(string[] args)
    {
        new IgdbProtoParser().ParseFile(
            @"d:\Downloads\igdbapi.proto",
            @"C:\Devel\PlayniteBackend\source\PlayniteServices\Controllers\IGDB\");
    }
}