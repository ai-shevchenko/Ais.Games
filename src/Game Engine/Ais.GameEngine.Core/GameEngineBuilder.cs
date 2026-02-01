using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.TimeSystem.Abstractions;
using Ais.GameEngine.Core.Settings;
using Ais.GameEngine.Core.TimeSystem;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

public sealed class GameEngineBuilder : IGameEngineBuilder
{
    private readonly IConfigurationManager _configuration;
    private readonly IServiceCollection _services;
    private readonly GameEngineBuilderContext _context;
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
        _context = new(_configuration);
    }

    public static GameEngineBuilder Create(params string[] args)
    {
        var builder = new GameEngineBuilder(args);
        return builder;
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
        var gameServices = _setting.ServiceProviderOptions is null 
            ? _services.BuildServiceProvider() 
            : _services.BuildServiceProvider(_setting.ServiceProviderOptions);

        var gameEngine = ActivatorUtilities.CreateInstance<GameEngine>(gameServices);

        return gameEngine;
    }

    private void Initialize(GameEngineBuilderSettings settings, out IServiceCollection services, out IConfigurationManager configuration)
    {
        configuration = new ConfigurationManager();
        services = new ServiceCollection();

        configuration.AddJsonFile("gamesettings.json", optional: true, reloadOnChange: true);
        if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") is { Length: > 0 } env)
        {
            configuration.AddJsonFile($"gamesettings.{env}.json", optional: true, reloadOnChange: true);
        }

        configuration.AddEnvironmentVariables();
        configuration.AddCommandLine(settings.Args);

        services.AddSingleton<IConfiguration>(configuration);
        services.AddOptions();
        
        services.AddSingleton<ITimerController, MainTimerController>();

        services.AddScoped<IGameLoopStateFactory, GameLoopStateFactory>();
        services.AddScoped<IGameLoopFactory, GameLoopFactory>();
        services.AddScoped<IHookFactory, HookFactory>();
        
        services.AddScoped<IGameLoopContextAccessor, GameLoopContextAccessor>();
        services.AddScoped<IGameLoopStateMachine, GameLoopStateMachine>();
        services.AddScoped<IGameLoopStateSource>(sp => sp.GetRequiredService<IGameLoopStateMachine>());
        services.AddScoped<IHooksSource, HooksSource>();

        services.AddTransient(sp => sp.GetRequiredService<ITimerController>().CreateChildTimer());

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