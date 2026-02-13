using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class OutOfBoundsEvent : IGameLoopEvent
{
    public required int PositionX { get; init; }
    public required int PositionY { get; init; }
    public required int MaxX { get; init; }
    public required int MaxY { get; init; }
    public required string EntityType { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
