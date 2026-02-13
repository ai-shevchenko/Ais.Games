namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Асинхронный интерфейс управления жизненным циклом игрового цикла.
///     Заменяет синхронные методы на асинхронные для правильного управления состоянием.
/// </summary>
public interface IGameLoopController
{
    /// <summary>
    ///     Получить текущее состояние игрового цикла.
    /// </summary>
    GameLoopState State { get; }

    /// <summary>
    ///     Проверить, запущен ли игровой цикл.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    ///     Проверить, приостановлен ли игровой цикл.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    ///     Запустить игровой цикл асинхронно.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Остановить игровой цикл асинхронно с graceful shutdown.
    /// </summary>
    /// <param name="timeout">Максимальное время ожидания корректного завершения. По умолчанию 5 сек.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task StopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Приостановить выполнение игрового цикла.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task PauseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Возобновить выполнение игрового цикла.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    Task ResumeAsync(CancellationToken cancellationToken = default);
}
