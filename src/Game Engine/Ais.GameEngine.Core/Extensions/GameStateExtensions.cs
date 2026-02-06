using Ais.GameEngine.Core.Internal.StateMachine.States;
using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.Extensions;

public static class GameStateExtensions
{
    public static Task Run(this IGameStateMachine stateMachine, CancellationToken cancellationToken = default)
    {
        return stateMachine.ChangeStateAsync<RunningState>(cancellationToken);
    }

    public static Task Pause(this IGameStateMachine stateMachine, CancellationToken cancellationToken = default)
    {
        return stateMachine.ChangeStateAsync<PauseState>(cancellationToken);
    }

    public static Task Stop(this IGameStateMachine stateMachine, CancellationToken cancellationToken = default)
    {
        return stateMachine.ChangeStateAsync<StoppingState>(cancellationToken);
    }
}
