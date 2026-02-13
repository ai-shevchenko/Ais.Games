using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие смены направления движения.
/// </summary>
internal sealed class DirectionChangedEvent : IGameLoopEvent
{
    public required int DirectionX { get; init; }
    public required int DirectionY { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
