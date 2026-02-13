using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class EntityClippingEvent : IGameLoopEvent
{
    public required int PositionX { get; init; }
    public required int PositionY { get; init; }
    public required string Entity1Type { get; init; }
    public required string Entity2Type { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
