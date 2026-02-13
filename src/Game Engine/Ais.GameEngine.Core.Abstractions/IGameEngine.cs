namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Игровой движок — главный оркестратор всех игровых циклов.
///     Управляет созданием, запуском, остановкой и координацией нескольких игровых циклов.
/// </summary>
public interface IGameEngine : IGameLoopManager, IDisposable
{
    /// <summary>
    ///     Текущее состояние игрового движка.
    /// </summary>
    EngineState State { get; }

    /// <summary>
    ///     Запустить все активные игровые циклы асинхронно.
    /// </summary>
    /// <param name="stoppingToken">Токен остановки движка.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task StartAsync(CancellationToken stoppingToken = default);

    /// <summary>
    ///     Остановить все игровые циклы асинхронно с graceful shutdown.
    /// </summary>
    /// <param name="timeout">Максимальное время ожидания корректного завершения. По умолчанию 10 сек.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task StopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Приостановить все активные игровые циклы.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task PauseAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Возобновить все приостановленные игровые циклы.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task ResumeAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
///     Состояние игрового движка.
/// </summary>
public enum EngineState
{
    /// <summary>
    ///     Движок не инициализирован.
    /// </summary>
    NotInitialized = 0,

    /// <summary>
    ///     Движок инициализирован, но не запущен.
    /// </summary>
    Idle = 1,

    /// <summary>
    ///     Движок запущен и работает.
    /// </summary>
    Running = 2,

    /// <summary>
    ///     Движок в процессе остановки.
    /// </summary>
    Stopping = 3,

    /// <summary>
    ///     Движок остановлен.
    /// </summary>
    Stopped = 4,

    /// <summary>
    ///     Движок завершился с ошибкой.
    /// </summary>
    Failed = 5
}
