using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Playnite.SDK;
using Playnite;
using MongoDB.Bson.Serialization;
using Microsoft.Extensions.DependencyInjection;
using PlayniteServices.Controllers.IGDB;
using PlayniteServices.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PlayniteServices
{
    public class Startup
    {
        private IConfiguration configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllers(options =>
            {
                options.Filters.Add(new ApiExceptionFilter());
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IncludeFields = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.Converters.Add(new ReleaseDateConverter());
            });

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.Configure<AppSettings>(configuration);
            services.AddSingleton(s => new UpdatableAppSettings(s.GetService<IOptionsMonitor<AppSettings>>()));
            services.AddSingleton<IgdbApi>();
            services.AddSingleton<PlayniteVersionFilter>();
            services.AddSingleton<ServiceKeyFilter>();
            services.AddSingleton<Discord>();
            services.AddSingleton<Addons>();
            services.AddSingleton<Database>();
            services.AddSingleton<Patreon>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                // Makes it so we can re-read request body in ApiExceptionFilter.
                context.Request.EnableBuffering();
                await next.Invoke();
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class Program
    {
        private static readonly Playnite.SDK.ILogger logger = LogManager.GetLogger();

        public static void Main(string[] args)
        {
            NLogLogger.ConfigureLogger();
            LogManager.Init(new NLogLogProvider());
            logger.Info("Server starting...");

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("customSettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddJsonFile("patreonTokens.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddJsonFile("twitchTokens.json", optional: true, reloadOnChange: true);

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            var app = builder.Build();
            startup.Configure(app, app.Environment);
            app.Run();
        }
    }
}
