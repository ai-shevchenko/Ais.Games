using Ais.GameEngine.Core.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Events;

internal sealed class PowerUpDurationMismatchEvent : IGameLoopEvent
{
    public required PowerUpType PowerUpType { get; init; }
    public required float ExpectedDuration { get; init; }
    public required float ActualDuration { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
