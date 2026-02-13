using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Events;

internal sealed class SnakeChainRepairedEvent : IGameLoopEvent
{
    public int NewLength { get; init; }
    public required string SourceLoopName { get; init; }
    public string? TargetLoopName { get; }
}
