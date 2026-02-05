using Ais.GameEngine.Extensions.SignalBus.Abstractions;

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
    private readonly Lock _sync = new();

    public GameState State { get; private set; } = GameState.None;

    public void SetResult(GameState state)
    {
        lock (_sync)
        {
            State = state;
        }
    }
}

internal sealed class GameOverSignal : ISignal
{
    public required bool IsWin { get; init; }
}
