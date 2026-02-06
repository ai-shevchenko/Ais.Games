using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.StateMachine;

internal sealed class GameStateProvider : IGameStateProvider
{
    private readonly IServiceProvider _gameServices;

    public GameStateProvider(IServiceProvider gameServices)
    {
        _gameServices = gameServices;
    }

    public T GetState<T>()
        where T : IGameState
    {
        return _gameServices.GetRequiredService<T>();
    }

    public IGameState GetState(Type stateType)
    {
        if (!typeof(IGameState).IsAssignableFrom(stateType))
        {
            throw new ArgumentException($"Type must implement {nameof(IGameState)}", nameof(stateType));
        }

        return (IGameState)_gameServices.GetRequiredService(stateType);
    }
}
