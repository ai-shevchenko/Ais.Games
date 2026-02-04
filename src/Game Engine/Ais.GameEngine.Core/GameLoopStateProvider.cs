using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateProvider : IGameLoopStateProvider
{
    private readonly IServiceProvider _gameServices;

    public GameLoopStateProvider(IServiceProvider gameServices)
    {
        _gameServices = gameServices;
    }

    public T GetState<T>()
        where T : IGameLoopState
    {
        return _gameServices.GetRequiredService<T>();
    }

    public IGameLoopState GetState(Type stateType)
    {
        if (!typeof(IGameLoopState).IsAssignableFrom(stateType))
        {
            throw new ArgumentException($"Type must implement {nameof(IGameLoopState)}", nameof(stateType));
        }

        return (IGameLoopState)_gameServices.GetRequiredService(stateType);
    }
}
