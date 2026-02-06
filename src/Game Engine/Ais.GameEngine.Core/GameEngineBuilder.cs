using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Internal.GameLoop;
using Ais.GameEngine.Core.Internal.HooksSystem;
using Ais.GameEngine.Core.Internal.ModulesSystem;
using Ais.GameEngine.Core.Internal.StateMachine;
using Ais.GameEngine.Core.Internal.TimeSystem;
using Ais.GameEngine.Core.Settings;
using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

public sealed class GameEngineBuilder : IGameEngineBuilder
{
    private readonly IConfigurationManager _configuration;
    private readonly GameEngineBuilderContext _context;
    private readonly List<IModuleEnricher> _enrichers = [];
    private readonly IKeyedModuleLoader _moduleLoader;
    private readonly IServiceCollection _services;
    private readonly GameEngineBuilderSettings _setting;

    public GameEngineBuilder(string[] args)
        : this(new GameEngineBuilderSettings { Args = args })
    {
    }

    public GameEngineBuilder(GameEngineBuilderSettings settings)
    {
        Initialize(settings, out var services, out var configuration);
        _setting = settings;
        _services = services;
        _configuration = configuration;
        _context = new GameEngineBuilderContext(_configuration);

        _moduleLoader = new ModuleLoader();
        _enrichers.Add(new ConfigurationModuleEnricher(_configuration));
    }

    public void ConfigureGameConfiguration(Action<IConfigurationBuilder> configure)
    {
        configure(_configuration);
    }

    public void ConfigureGameLogging(Action<GameEngineBuilderContext, ILoggingBuilder> configure)
    {
        _services.AddLogging(builder => configure(_context, builder));
    }

    public void AddModuleEnricher(IModuleEnricher enricher)
    {
        _enrichers.Add(enricher);
    }

    public void AddModuleEnricher(Action<IKeyedModuleLoader> enricher)
    {
        _enrichers.Add(new InlineModuleEnricher(enricher));
    }

    public void ConfigureGameServices(Action<GameEngineBuilderContext, IServiceCollection> configure)
    {
        configure(_context, _services);
    }

    public IGameEngine Build()
    {
        foreach (var enricher in _enrichers)
        {
            enricher.Enrich(_moduleLoader);
        }

        foreach (var module in _moduleLoader.GetLoadedModules())
        {
            module.ConfigureGameServices(_services, _configuration);
        }

        var factory = new GameLoopFactory(_services, _configuration, _moduleLoader);
        var engine = new Internal.GameLoop.GameEngine(factory);
        return engine;
    }

    public static GameEngineBuilder Create(params string[] args)
    {
        var builder = new GameEngineBuilder(args);
        return builder;
    }

    public static GameEngineBuilder Create(GameEngineBuilderSettings settings)
    {
        var builder = new GameEngineBuilder(settings);
        return builder;
    }

    public static GameEngineBuilder Create(Action<GameEngineBuilderSettings> configure)
    {
        var settings = new GameEngineBuilderSettings();
        configure(settings);
        var builder = new GameEngineBuilder(settings);
        return builder;
    }

    private static void Initialize(
        GameEngineBuilderSettings settings,
        out IServiceCollection services,
        out IConfigurationManager configuration)
    {
        configuration = new ConfigurationManager();
        services = new ServiceCollection();

        configuration.AddJsonFile("gamesettings.json", true, true);
        if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") is { Length: > 0 } env)
        {
            configuration.AddJsonFile($"gamesettings.{env}.json", true, true);
        }

        configuration
            .AddEnvironmentVariables()
            .AddCommandLine(settings.Args);

        services
            .AddSingleton<IConfiguration>(configuration)
            .AddOptions();

        services.AddStateMachine();
        services.AddTimeSystem();
        services.AddHooksSystem();

        var engineSettings = configuration.GetSection(nameof(GameEngineSettings));
        if (engineSettings.Exists())
        {
            services.Configure<GameEngineSettings>(engineSettings);
        }
        else
        {
            services.Configure<GameEngineSettings>(_ => { });
        }
    }
}
