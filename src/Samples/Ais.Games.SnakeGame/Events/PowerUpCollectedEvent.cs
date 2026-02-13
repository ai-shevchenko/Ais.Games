using Ais.GameEngine.Core.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие сбора power-up.
/// </summary>
internal sealed class PowerUpCollectedEvent : IGameLoopEvent
{
    public required PowerUpType PowerUpType { get; init; }
    public required float EffectDuration { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
