using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие съедения еды.
/// </summary>
internal sealed class FoodEatenEvent : IGameLoopEvent
{
    public required int ScoreGained { get; init; }
    public required int TotalScore { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
