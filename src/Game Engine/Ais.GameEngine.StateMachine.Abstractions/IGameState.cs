namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Состояние игрового цикла
/// </summary>
public interface IGameState
{
    /// <summary>
    ///     Метод входа в новое состояние
    /// </summary>
    /// <param name="context">Контекст игрового цикла</param>
    /// <param name="stoppingToken">Токен отмены</param>
    Task EnterAsync(GameContext context, CancellationToken stoppingToken = default);

    /// <summary>
    ///     Метод выполнения состояния
    /// </summary>
    /// <param name="context">Контекст игрового цикла</param>
    /// <param name="stoppingToken">Токен отмены</param>
    Task ExecuteAsync(GameContext context, CancellationToken stoppingToken = default);

    /// <summary>
    ///     Метод выхода из состояния
    /// </summary>
    /// <param name="context">Контекст игрового цикла</param>
    /// <param name="stoppingToken">Токен отмены</param>
    Task ExitAsync(GameContext context, CancellationToken stoppingToken = default);
}
