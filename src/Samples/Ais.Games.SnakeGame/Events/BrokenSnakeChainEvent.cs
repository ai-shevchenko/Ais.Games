using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class BrokenSnakeChainEvent : IGameLoopEvent
{
    public required int SegmentCount { get; init; }
    public required int MaxOrder { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
