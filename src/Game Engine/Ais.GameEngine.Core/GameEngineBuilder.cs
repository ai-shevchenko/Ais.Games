using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Internal.DI;
using Ais.GameEngine.Core.Internal.GameLoop;
using Ais.GameEngine.Core.Internal.Helpers;
using Ais.GameEngine.Core.Internal.ModulesSystem;
using Ais.GameEngine.Modules.Abstractions;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

/// <summary>
/// Построитель для создания и конфигурирования экземпляра игрового движка.
/// Использует паттерн Builder для удобного и гибкого создания движка.
/// </summary>
public sealed class GameEngineBuilder : IGameEngineBuilder
{
    private readonly GameEngineBuilderContext _context;
    private readonly LoggingConfigurator _loggingConfigurator;
    private readonly ModuleEnricherManager _moduleEnricherManager;
    private readonly IModuleLoader _moduleLoader;
    private readonly IConfigurationManager _configuration;
    private readonly IServiceCollection _services;
    private readonly GameEngineBuilderSettings _settings;
    private readonly ServicesConfigurator _servicesConfigurator;

    public GameEngineBuilder(string[] args)
        : this(new GameEngineBuilderSettings { Args = args })
    {
    }

    public GameEngineBuilder(GameEngineBuilderSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        _configuration = GameEngineInitializer.InitializeConfiguration(settings);
        _context = new GameEngineBuilderContext(_configuration);

        _services = new ServiceCollection();
        GameEngineInitializer.InitializeServices(_services, _configuration);

        _moduleLoader = settings.ModuleLoader ?? new ModuleLoader();
        _moduleEnricherManager = new ModuleEnricherManager();
        _loggingConfigurator = new LoggingConfigurator();
        _servicesConfigurator = new ServicesConfigurator();

        _moduleEnricherManager.AddEnricher(new ConfigurationModuleEnricher(_configuration));
    }

    public void ConfigureGameConfiguration(Action<IConfigurationBuilder> configure)
    {
        configure(_configuration);
    }

    public void ConfigureGameLogging(Action<GameEngineBuilderContext, ILoggingBuilder> configure)
    {
        _loggingConfigurator.Configure(configure);
    }

    public void AddModuleEnricher(IModuleEnricher enricher)
    {
        _moduleEnricherManager.AddEnricher(enricher);
    }

    public void AddModuleEnricher(Action<IModuleLoader> enricher)
    {
        _moduleEnricherManager.AddEnricher(enricher);
    }

    public void ConfigureGameServices(Action<GameEngineBuilderContext, IServiceCollection> configure)
    {
        _servicesConfigurator.AddConfigurator(configure);
    }

    public IGameEngine Build()
    {
        _moduleEnricherManager.EnrichAll(_moduleLoader);

        _loggingConfigurator.Apply(_services, _context);

        _servicesConfigurator.ApplyAll(_services, _context);

        foreach (var module in _moduleLoader.GetLoadedModules("Default"))
        {
            module.ConfigureGameServices(_services, _configuration);
        }

        var builder = new ContainerBuilder();
        builder.Populate(_services);
        var container = builder.Build();

        var scopeFactory = new AutofacServiceScopeFactory(container);

        var factory = new GameLoopFactory(
            _configuration,
            _moduleLoader,
            container.Resolve<ILogger<GameLoopFactory>>(),
            scopeFactory);

        var engine = new Internal.GameLoop.GameEngine(factory);
        return engine;
    }

    public static GameEngineBuilder Create(params string[] args)
    {
        return new GameEngineBuilder(args);
    }

    public static GameEngineBuilder Create(GameEngineBuilderSettings settings)
    {
        return new GameEngineBuilder(settings);
    }

    public static GameEngineBuilder Create(Action<GameEngineBuilderSettings> configure)
    {
        var settings = new GameEngineBuilderSettings();
        configure(settings);
        return new GameEngineBuilder(settings);
    }
}
