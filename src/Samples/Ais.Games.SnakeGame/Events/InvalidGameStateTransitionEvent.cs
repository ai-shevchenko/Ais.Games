using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class InvalidGameStateTransitionEvent : IGameLoopEvent
{
    public required GameState FromState { get; init; }
    public required GameState ToState { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
