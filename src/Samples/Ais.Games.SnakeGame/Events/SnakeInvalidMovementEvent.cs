using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class SnakeInvalidMovementEvent : IGameLoopEvent
{
    public required int AttemptedDirectionX { get; init; }
    public required int AttemptedDirectionY { get; init; }
    public required int CurrentDirectionX { get; init; }
    public required int CurrentDirectionY { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
