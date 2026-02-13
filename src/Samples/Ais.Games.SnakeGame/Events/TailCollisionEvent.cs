using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие столкновения с хвостом.
/// </summary>
internal sealed class TailCollisionEvent : IGameLoopEvent
{
    public required int PositionX { get; init; }
    public required int PositionY { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
