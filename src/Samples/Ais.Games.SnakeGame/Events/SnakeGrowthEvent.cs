using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие роста змейки.
/// </summary>
internal sealed class SnakeGrowthEvent : IGameLoopEvent
{
    public required int NewLength { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
