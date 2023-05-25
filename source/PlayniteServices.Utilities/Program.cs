namespace PlayniteServices.Utilities;

public partial class Program
{
    public static void Main(string[] args)
    {
        new IgdbProtoParser().ParseFile(
            @"c:\Devel\PlayniteBackend\source\igdbapi.proto",
            @"C:\Devel\PlayniteBackend\source\PlayniteServices\Controllers\IGDB\");
    }
}