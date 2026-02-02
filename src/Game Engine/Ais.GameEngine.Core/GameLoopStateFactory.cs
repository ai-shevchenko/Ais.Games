using Ais.GameEngine.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateFactory : IGameLoopStateFactory
{
    private readonly IServiceProvider _gameServices;

    public GameLoopStateFactory(IServiceProvider gameServices)
    {
        _gameServices = gameServices;
    }

    public T CreateState<T>()
        where T : IGameLoopState
    {
        return ActivatorUtilities.CreateInstance<T>(_gameServices);
    }

    public IGameLoopState CreateState(Type stateType)
    {
        if (!typeof(IGameLoopState).IsAssignableFrom(stateType))
        {
            throw new ArgumentException($"Type must implement {nameof(IGameLoopState)}", nameof(stateType));
        }

        return (IGameLoopState)ActivatorUtilities.CreateInstance(_gameServices, stateType);
    }
}
