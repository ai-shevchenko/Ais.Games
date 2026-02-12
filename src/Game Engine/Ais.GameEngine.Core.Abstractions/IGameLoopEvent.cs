namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Событие между игровыми циклами
/// </summary>
public interface IGameLoopEvent
{
    /// <summary>
    ///     Имя исходящего цикла
    /// </summary>
    string SourceLoopName { get; }

    /// <summary>
    ///     Имя целевого цикла (null = broadcast)
    /// </summary>
    string? TargetLoopName { get; }
}
