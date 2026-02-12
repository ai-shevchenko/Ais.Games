namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Событие изменения состояния игрового цикла.
/// </summary>
public class GameLoopStateChangedEvent : IGameLoopEvent
{
    /// <inheritdoc />
    public required string SourceLoopName { get; init; }

    /// <inheritdoc />
    public string? TargetLoopName { get; init; }

    /// <summary>
    ///     Состояние игрового цикла.
    /// </summary>
    public GameLoopState State { get; init; }
}
