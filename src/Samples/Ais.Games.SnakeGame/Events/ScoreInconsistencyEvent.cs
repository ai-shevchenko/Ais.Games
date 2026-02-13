using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class ScoreInconsistencyEvent : IGameLoopEvent
{
    public required int ExpectedScore { get; init; }
    public required int ActualScore { get; init; }
    public required int FruitsEaten { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
