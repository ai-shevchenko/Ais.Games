using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopContextAccessor : IGameLoopContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> _gameLoopContextCurrent = new();

    /// <inheritdoc/>
    public GameLoopContext? CurrentContext
    {
        get
        {
            return _gameLoopContextCurrent.Value?.Context;
        }
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
        public GameLoopContext? Context;
    }
}