using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.States;

namespace Ais.GameEngine.Core.Extensions;

public static class GameStateExtensions
{
    public static Task Run(this IGameLoopStateMachine stateMachine, CancellationToken cancellationToken = default)
    {
        return stateMachine.ChangeStateAsync<RunningState>(cancellationToken);
    }

    public static Task Pause(this IGameLoopStateMachine stateMachine, CancellationToken cancellationToken = default)
    {
        return stateMachine.ChangeStateAsync<PauseState>(cancellationToken);
    }

    public static Task Stop(this IGameLoopStateMachine stateMachine, CancellationToken cancellationToken = default)
    {
        return stateMachine.ChangeStateAsync<StoppingState>(cancellationToken);
    }
}
