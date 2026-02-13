using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

/// <summary>
///     Событие начала игры.
/// </summary>
internal sealed class GameStartedEvent : IGameLoopEvent
{
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
