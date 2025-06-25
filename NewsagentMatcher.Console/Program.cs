using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsagentMatcher.Console;
using NewsagentMatcher.Core.Configuration;
using NewsagentMatcher.Core.Factories;
using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.MatchStrategies;
using NewsagentMatcher.Core.Models;
using NewsagentMatcher.Core.Providers;
using NewsagentMatcher.Core.Services;
using Serilog;
using Serilog.Events;

try
{
    ConfigureLogging();
    LogBox.BoxedInformation("APPLICATION STARTING");

    using var cts = SetupCancellationToken();

    var host = BuildHost(args);
    var newsagentService = host.Services.GetRequiredService<INewsagentService>();
    var chainSettings = host.Services.GetRequiredService<IOptions<NewsagentChainSettings>>().Value;

    await RunValidationProcess(newsagentService, Log.Logger, chainSettings, cts.Token);
    await host.StopAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Application stopped");
    Log.CloseAndFlush();
}

static void ConfigureLogging()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
        .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
        .MinimumLevel.Override("System.Net.Http.Resilience", LogEventLevel.Warning)
        .MinimumLevel.Override("Polly", LogEventLevel.Warning)
        .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/log.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
        .CreateLogger();
}

static IHost BuildHost(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureAppConfiguration(ConfigureAppSettings)
        .ConfigureServices(ConfigureServices)
        .Build();
}

static void ConfigureAppSettings(IConfigurationBuilder config)
{
    config.AddJsonFile("appsettings.json", optional: false);
    config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true);
}

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    ConfigureSettings(context, services);
    ConfigureHttpClients(context, services);
    ConfigureApplicationServices(services);
}

static void ConfigureSettings(HostBuilderContext context, IServiceCollection services)
{
    services.Configure<ZineCoSettings>(context.Configuration.GetSection(ZineCoSettings.SectionName));
    services.Configure<NewsagentChainSettings>(context.Configuration.GetSection(NewsagentChainSettings.SectionName));
}

static void ConfigureHttpClients(HostBuilderContext context, IServiceCollection services)
{
    services.AddHttpClient("NewsagentClient")
        .AddStandardResilienceHandler();

    services.AddHttpClient<IZineCoDataProvider, ZineCoApiProvider>(client =>
    {
        var settings = context.Configuration.GetSection(ZineCoSettings.SectionName).Get<ZineCoSettings>();
        client.Timeout = TimeSpan.FromSeconds(settings?.TimeoutSeconds ?? 30);
        client.BaseAddress = new Uri(settings?.Endpoint ?? throw new InvalidOperationException("ZineCo endpoint not configured"));
    });
}

static void ConfigureApplicationServices(IServiceCollection services)
{
    services.AddSingleton<Func<string, INewsagentProvider>>(sp =>
    {
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var settings = sp.GetRequiredService<IOptions<NewsagentChainSettings>>();
        var logger = sp.GetRequiredService<ILogger<NewsagentApiProvider>>();

        return (string chainId) => new NewsagentApiProvider(
            factory.CreateClient("NewsagentClient"),
            settings,
            logger,
            chainId);
    });

    services.AddSingleton<INewsagentMatchStrategy, SuperNewsMatchStrategy>();
    services.AddSingleton<INewsagentMatchStrategy, NewsInWordsMatchStrategy>();
    services.AddSingleton<INewsagentMatchStrategy, AdventureNewsMatchStrategy>();
    services.AddSingleton<INewsagentMatchStrategyFactory, NewsagentMatchStrategyFactory>();
    services.AddSingleton<INewsagentService, NewsagentService>();
}

static CancellationTokenSource SetupCancellationToken()
{
    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => e.Cancel = true;
    return cts;
}

static async Task RunValidationProcess(INewsagentService newsagentService, Serilog.ILogger logger, NewsagentChainSettings settings, CancellationToken cancellationToken)
{
    LogBox.BoxedInformation("STARTING VALIDATION PROCESS");

    var zineCoAgents = await GetZineCoAgents(newsagentService, logger, cancellationToken);
    if (zineCoAgents == null) return;

    //var chainsToValidate = new[] { "SUP", "ADV", "NIW" };
    var chainsToValidate = settings.Chains.Keys;
    foreach (var chainId in chainsToValidate)
    {
        await ValidateChain(chainId, newsagentService, zineCoAgents, logger, cancellationToken);
    }

    LogBox.BoxedInformation("VALIDATION COMPLETED");
}

static async Task<List<ZineCoNewsagent>?> GetZineCoAgents(INewsagentService newsagentService, Serilog.ILogger logger, CancellationToken cancellationToken)
{
    try
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        return (await newsagentService.GetZineCoNewsagentsAsync(linkedCts.Token)).ToList();
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
        logger.Warning("Operation cancelled by user");
        return null;
    }
    catch (OperationCanceledException)
    {
        logger.Warning("ZineCo API request timed out");
        return null;
    }
    catch (Exception ex)
    {
        logger.Error(ex, "Failed to retrieve ZineCo agents");
        return null;
    }
}

static async Task ValidateChain(string chainId, INewsagentService newsagentService,
    List<ZineCoNewsagent> zineCoAgents, Serilog.ILogger logger, CancellationToken cancellationToken)
{
    LogBox.BoxedInformation($"VALIDATING {chainId} AGENTS");

    var zineCoAgentsByChain = zineCoAgents.Where(a => a.ChainId == chainId).ToList();
    if (!zineCoAgentsByChain.Any())
    {
        logger.Warning("No agents found for chain {Chain}", chainId);
        return;
    }

    var newsAgents = await newsagentService.GetNewsagentsAsync(chainId, cancellationToken);
    if (newsAgents == null) return;

    try
    {
        var results = await newsagentService.ValidateByChainAsync(
            chainId,
            zineCoAgentsByChain,
            newsAgents,
            cancellationToken);

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        foreach (var (newsagent, result) in results)
        {
            logger.Information("[{ChainId}]: {Status} {Message}",
                chainId,
                newsagent.Name,
                result.IsValid ? "✅" : "❌",
                result.Message);
        }
    }
    catch (Exception ex)
    {
        logger.Error(ex, "Error validating {AgentName}", newsAgents.FirstOrDefault()?.Name);
    }
}