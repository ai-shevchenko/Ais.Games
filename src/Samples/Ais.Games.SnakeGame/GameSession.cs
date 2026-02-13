using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.Games.SnakeGame.Events;

namespace Ais.Games.SnakeGame;

internal enum GameState
{
    None,
    Start,
    Won,
    Lost,
    Exit
}

internal sealed class GameSession
{
    private static readonly Dictionary<GameState, HashSet<GameState>> _validTransitions = new(5)
    {
        { GameState.None, [GameState.Start, GameState.Exit] },
        { GameState.Start, [GameState.Won, GameState.Lost, GameState.Exit] },
        { GameState.Won, [GameState.None, GameState.Exit] },
        { GameState.Lost, [GameState.None, GameState.Exit] },
        { GameState.Exit, [] }
    };

    private readonly Lock _sync = new();
    private IGameContextAccessor? _accessor;
    private IGameLoopEventBus? _eventBus;

    public GameState State { get; private set; } = GameState.None;

    public void SetEventBusAndAccessor(IGameLoopEventBus eventBus, IGameContextAccessor accessor)
    {
        _eventBus = eventBus;
        _accessor = accessor;
    }

    public void SetResult(GameState state)
    {
        lock (_sync)
        {
            ValidateStateTransition(State, state);
            State = state;
        }
    }

    private void ValidateStateTransition(GameState from, GameState to)
    {
        if (_validTransitions.ContainsKey(from) && !_validTransitions[from].Contains(to))
        {
            _ = _eventBus?.PublishAsync(new InvalidGameStateTransitionEvent
            {
                SourceLoopName = _accessor?.CurrentContext?.LoopName ?? "unknown", FromState = from, ToState = to
            });
        }
    }
}
