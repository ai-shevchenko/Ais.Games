using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopFactory : IGameLoopFactory
{
    private readonly IServiceProvider _gameServices;

    public GameLoopFactory(IServiceProvider gameServices)
    {
        _gameServices = gameServices;
    }

    public IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var loopScope = _gameServices.CreateScope();

        var contextAccessor = loopScope.ServiceProvider
            .GetRequiredService<IGameLoopContextAccessor>();

        var settings = ActivatorUtilities.CreateInstance<GameLoopBuilderSettings>(loopScope.ServiceProvider);
        ConfigureContext(name, settings, contextAccessor);

        configure(settings);
        
        return ActivatorUtilities.CreateInstance<GameLoop>(loopScope.ServiceProvider, loopScope);
    }

    public IGameLoop CreateGameLoop(string name, GameLoopBuilderSettings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var loopScope = _gameServices.CreateScope();

        var contextAccessor = loopScope.ServiceProvider
            .GetRequiredService<IGameLoopContextAccessor>();

        ConfigureContext(name, settings, contextAccessor);

        return ActivatorUtilities.CreateInstance<GameLoop>(loopScope.ServiceProvider, loopScope);
    }

    private static void ConfigureContext(string name, GameLoopBuilderSettings settings, IGameLoopContextAccessor contextAccessor)
    {
        var context = new GameLoopContext
        {
            Hooks = settings.Hooks,
            LoopName = name
        };

        contextAccessor.CurrentContext = context;
    }
}