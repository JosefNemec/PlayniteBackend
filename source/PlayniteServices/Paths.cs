using System.IO;

namespace PlayniteServices
{
    public class ServicePaths
    {
        public static string ExecutingDirectory { get; }

        static ServicePaths()
        {
            ExecutingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        }
    }
}

namespace Playnite
{
    public static class PlaynitePaths
    {
        public static string LogFile { get; }

        static PlaynitePaths()
        {
            LogFile = Path.Combine(PlayniteServices.ServicePaths.ExecutingDirectory, "playnite.log");
        }
    }
}
