using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class SnakeLengthMismatchEvent : IGameLoopEvent
{
    public required int ExpectedLength { get; init; }
    public required int ActualLength { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; init; }
}
