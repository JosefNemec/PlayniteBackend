using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Playnite;
using ILogger = Playnite.ILogger;

namespace PlayniteServices;

public class Program
{
    private static readonly ILogger logger = LogManager.GetLogger();
    public static readonly HttpClient HttpClient = new ();

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public static async Task Main(string[] args)
    {
        logger.Info("Server starting...");

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("customSettings.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile("patreonTokens.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile("twitchTokens.json", optional: true, reloadOnChange: true);

        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        ConfigureApp(app);

        var settings = app.Services.GetService<UpdatableAppSettings>()!;
        if (settings.Settings.Addons?.AutoUpdate == true)
        {
            var addons = app.Services.GetService<Addons.AddonsManager>()!;
            addons.StartUpdateChecker();
        }

        if (settings.Settings.Discord?.BotEnabled == true)
        {
            var discord = app.Services.GetService<Discord.DiscordManager>()!;
            await discord.Init();
        }

        app.Run();
    }
}
