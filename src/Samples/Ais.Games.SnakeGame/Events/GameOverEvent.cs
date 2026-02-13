using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие конца игры (выигрыш или проигрыш).
/// </summary>
internal sealed class GameOverEvent : IGameLoopEvent
{
    public required bool IsWin { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
