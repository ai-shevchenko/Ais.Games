using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Extensions;
using Ais.GameEngine.Core.Hooks;
using Ais.GameEngine.Core.Interceptors;
using Ais.GameEngine.Core.Settings;
using Ais.GameEngine.Core.States;
using Ais.GameEngine.Core.TimeSystem;
using Ais.GameEngine.TimeSystem.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

public sealed class GameEngineBuilder : IGameEngineBuilder
{
    private readonly IConfigurationManager _configuration;
    private readonly GameEngineBuilderContext _context;
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
    }

    public void ConfigureGameConfiguration(Action<IConfigurationBuilder> configure)
    {
        configure(_configuration);
    }

    public void ConfigureGameLogging(Action<GameEngineBuilderContext, ILoggingBuilder> configure)
    {
        _services.AddLogging(builder => configure(_context, builder));
    }

    public void ConfigureGameServices(Action<GameEngineBuilderContext, IServiceCollection> configure)
    {
        configure(_context, _services);
    }

    public IGameEngine Build()
    {
        var provider = _setting.ServiceProviderOptions is null
            ? _services.BuildServiceProvider()
            : _services.BuildServiceProvider(_setting.ServiceProviderOptions);

        var engine = new GameEngine(provider);
        _services.AddSingleton<IGameEngine>(engine);

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

        configuration.AddEnvironmentVariables();
        configuration.AddCommandLine(settings.Args);

        services.AddSingleton<IConfiguration>(configuration);
        services.AddOptions();

        services.AddSingleton<ITimerController, TimerController>();

        services.AddScoped<IGameLoopStateProvider, GameLoopStateProvider>();
        services.AddScoped<IHooksProvider, HooksProvider>();

        services.AddScoped<IGameLoopContextAccessor, GameLoopContextAccessor>();
        services.AddScoped<IGameLoopStateMachine, GameLoopStateMachine>();
        services.AddScoped<IHooksProvider, HooksProvider>();

        services.AddTransient(sp => sp.GetRequiredService<ITimerController>().CreateChildTimer());

        services.AddState<InitializeState>();
        services.AddState<RunningState>();
        services.AddState<PauseState>();
        services.AddState<StoppingState>();

        services.AddStateInterceptor<LoggingInterceptor>();

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
