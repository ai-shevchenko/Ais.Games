using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.Internal.StateMachine;

internal sealed class GameContextAccessor : IGameContextAccessor
{
    private readonly AsyncLocal<ContextHolder> _gameLoopContextCurrent = new();

    /// <inheritdoc />
    public GameContext? CurrentContext
    {
        get => _gameLoopContextCurrent.Value?.Context;
        set
        {
            _gameLoopContextCurrent.Value?.Context = null;

            if (value != null)
            {
                _gameLoopContextCurrent.Value = new ContextHolder { Context = value };
            }
        }
    }

    private sealed class ContextHolder
    {
        public GameContext? Context;
    }
}
