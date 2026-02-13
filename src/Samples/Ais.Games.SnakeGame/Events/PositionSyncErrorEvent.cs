using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class PositionSyncErrorEvent : IGameLoopEvent
{
    public required int ExpectedX { get; init; }
    public required int ExpectedY { get; init; }
    public required int ActualX { get; init; }
    public required int ActualY { get; init; }
    public required string EntityType { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
