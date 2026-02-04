namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Машина состояний для управления игровым циклом
/// </summary>
public interface IGameLoopStateMachine : IDisposable
{
    /// <summary>
    ///     Текущее состояние
    /// </summary>
    IGameLoopState? CurrentState { get; }

    /// <summary>
    ///     Изменить статус
    /// </summary>
    /// <typeparam name="T">Тип статуса</typeparam>
    /// <param name="stoppingToken">Токен отмены</param>
    /// <returns></returns>
    Task ChangeStateAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState;

    /// <summary>
    ///     Запустить машину состояний с указанного статуса
    /// </summary>
    /// <typeparam name="T">Тип статус</typeparam>
    /// <param name="stoppingToken">Токен отмены</param>
    /// <returns></returns>
    Task StartAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState;

    /// <summary>
    ///     Остановить выполнение машины
    /// </summary>
    Task StopAsync();
}
