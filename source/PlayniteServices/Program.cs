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
using Playnite;
using MongoDB.Bson.Serialization;

namespace PlayniteServices
{
    public class Program
    {
        private static readonly ILogger logger = LogManager.GetLogger();

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

            Database.Instance = (Database)host.Services.GetService(typeof(Database));
            Addons.Instance = (Addons)host.Services.GetService(typeof(Addons));
            Discord.Instance = (Discord)host.Services.GetService(typeof(Discord));
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
