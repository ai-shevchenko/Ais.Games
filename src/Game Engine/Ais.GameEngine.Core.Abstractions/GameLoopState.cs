namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Состояние игрового цикла для конечного автомата.
/// </summary>
public enum GameLoopState
{
    /// <summary>
    /// Цикл остановлен и не запущен.
    /// </summary>
    Stopped = 0,

    /// <summary>
    /// Цикл в процессе инициализации.
    /// </summary>
    Initializing = 1,

    /// <summary>
    /// Цикл работает.
    /// </summary>
    Running = 2,

    /// <summary>
    /// Цикл приостановлен.
    /// </summary>
    Paused = 3,

    /// <summary>
    /// Цикл в процессе остановки.
    /// </summary>
    Stopping = 4,

    /// <summary>
    /// Цикл завершился с ошибкой.
    /// </summary>
    Failed = 5
}
