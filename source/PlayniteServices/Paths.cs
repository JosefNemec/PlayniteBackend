using System.IO;

namespace Playnite
{
    public static class PlaynitePaths
    {
        public const string CustomConfigFileName = "customSettings.json";
        public const string PatreonConfigFileName = "patreonTokens.json";
        public const string TwitchConfigFileName = "twitchTokens.json";

        public static string LogFile { get; private set; }
        public static string ExecutingDirectory { get; private set; }
        public static string RuntimeDataDir { get; private set; }

        static PlaynitePaths()
        {
            ExecutingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
            RuntimeDataDir = ExecutingDirectory;
            LogFile = Path.Combine(RuntimeDataDir, "playnite.log");
        }

        public static void SetLogDir(string dir)
        {
            LogFile = Path.Combine(dir, "playnite.log");
        }

        public static void SetRuntimeDataDir(string dir)
        {
            RuntimeDataDir = dir;
        }
    }
}
