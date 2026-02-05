using Ais.GameEngine.Extensions.SignalBus.Abstractions;

namespace Ais.Games.SnakeGame;

internal enum GameResult
{
    None,
    Won,
    Lost,
    Exit
}

/// <summary>
/// Глобальное состояние игровой сессии, используется для связи между геймлупами.
/// </summary>
internal sealed class GameSession
{
    private readonly object _sync = new();

    public GameResult Result { get; private set; } = GameResult.None;

    public void SetResult(GameResult result)
    {
        lock (_sync)
        {
            Result = result;
        }
    }
}

/// <summary>
/// Сигнал окончания игры, публикуется из CollisionSystem.
/// </summary>
internal sealed class GameOverSignal : ISignal
{
    public required bool IsWin { get; init; }
}

