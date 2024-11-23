using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using Playnite;
using ILogger = Playnite.ILogger;

namespace Playnite.Backend;

public class Program
{
    private static readonly ILogger logger = LogManager.GetLogger();
    public static readonly HttpClient HttpClient = new ();
    public static readonly Version? Version = Assembly.GetExecutingAssembly().GetName().Version;

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.AddControllers(options =>
        {
            options.Filters.Add(new ApiExceptionFilter());
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            if (builder.Environment.IsDevelopment())
            {
                loggingBuilder.AddDebug();
            }
        });

        services.Configure<AppSettings>(configuration);
        services.AddSingleton(s => new UpdatableAppSettings(s.GetService<IOptionsMonitor<AppSettings>>()!));
        services.AddSingleton<IGDB.IgdbManager>();
        services.AddSingleton<PlayniteVersionFilter>();
        services.AddSingleton<ServiceKeyFilter>();
        services.AddSingleton<Discord.DiscordManager>();
        services.AddSingleton<Addons.AddonsManager>();
        services.AddSingleton<Database>();
        services.AddSingleton<Patreon.PatreonManager>();
    }

    public static void ConfigureApp(IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            // Makes it so we can re-read request body in ApiExceptionFilter.
            context.Request.EnableBuffering();
            await next.Invoke();
        });

        app.UseRouting();
        app.UseForwardedHeaders();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private static void Run(string[] args)
    {
        LogManager.SetLogManager(new NLogLogProvider());
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddCommandLine(args);
        var runtimeDataDir = builder.Configuration.GetValue<string>("RuntimeDataDir");
        if (!runtimeDataDir.IsNullOrWhiteSpace() && Directory.Exists(runtimeDataDir))
        {
            builder.Configuration.SetBasePath(runtimeDataDir);
            PlaynitePaths.SetRuntimeDataDir(runtimeDataDir);
            PlaynitePaths.SetLogDir(runtimeDataDir);
        }

        builder.Configuration.AddJsonFile(PlaynitePaths.CustomConfigFileName, optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile(PlaynitePaths.PatreonConfigFileName, optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile(PlaynitePaths.TwitchConfigFileName, optional: true, reloadOnChange: true);

        logger.Info($"Server {Version?.ToString(2)} starting...");
        logger.Info($"Using runtime dir: {PlaynitePaths.RuntimeDataDir}");

        ConfigureServices(builder);
        var app = builder.Build();
        ConfigureApp(app);

        var settings = app.Services.GetService<UpdatableAppSettings>()!;
        settings.SettingsChanged += (_, _) =>
        {
            NLogLogProvider.TraceLoggingEnabled = settings.Settings.TraceLogEnabled;
        };

        NLogLogProvider.TraceLoggingEnabled = settings.Settings.TraceLogEnabled;

        if (settings.Settings.Addons?.AutoUpdate == true)
        {
            var addons = app.Services.GetService<Addons.AddonsManager>()!;
            addons.StartUpdateChecker();
        }

        if (settings.Settings.Discord?.BotEnabled == true)
        {
            var discord = app.Services.GetService<Discord.DiscordManager>()!;
#pragma warning disable CS4014
            discord.Init();
#pragma warning restore CS4014
        }

        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            eventArgs.SetObserved();
            logger.Error(eventArgs.Exception, "Unhandled task exception:");
        };

        app.Run();
    }

    public static void Main(string[] args)
    {
        try
        {
            Run(args);
        }
        catch (Exception e)
        {
            logger.Error(e, "Server startup failed.");
        }
    }
}
