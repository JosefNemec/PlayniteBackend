using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using PlayniteServices.Databases;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Playnite.SDK;
using LiteDB;
using Playnite;

namespace PlayniteServices
{
    public class Program
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        public static Database Database { get; private set; }
        public static LiteDatabase AddonsDatabase { get; private set; }
        public static LiteCollection<AddonManifestBase> AddonsCollection { get; private set; }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("customSettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("patreonTokens.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("twitchTokens.json", optional: true, reloadOnChange: true);
                })
                .Build();

            Database = new Database(Database.Path);

            var addonDbPath = Startup.Configuration.GetSection(nameof(AppSettings.Addons)).GetSection(nameof(Addons.DatabasePath)).Value;
            if (!Playnite.Common.Paths.IsFullPath(addonDbPath))
            {
                addonDbPath = Path.Combine(Paths.ExecutingDirectory, addonDbPath);
            }

            AddonsDatabase = new LiteDatabase($"Filename={addonDbPath};Mode=Exclusive");
            AddonsCollection = AddonsDatabase.GetCollection<AddonManifestBase>();
            BsonMapper.Global.Entity<AddonManifestBase>().Id(a => a.AddonId);
            AddonsCollection.EnsureIndex(a => a.AddonId);
            return host;
        }

        public static void Main(string[] args)
        {
            NLogLogger.ConfigureLogger();
            LogManager.Init(new NLogLogProvider());
            logger.Info("Server starting...");
            BuildWebHost(args).Run();
        }
    }
}
