using Ais.GameEngine.Core.States;
using Ais.GameEngine.StateMachine.Abstractions;

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
