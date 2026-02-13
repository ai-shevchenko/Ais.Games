using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class SystemExecutionTimeAnomalyEvent : IGameLoopEvent
{
    public required string SystemName { get; init; }
    public required float ExecutionTimeMs { get; init; }
    public required float ThresholdMs { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
