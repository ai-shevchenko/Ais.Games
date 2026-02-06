namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Перехватчик состояния игрового цикла
/// </summary>
public interface IGameStateInterceptor
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task BeforeEnterAsync(GameContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task AfterEnterAsync(GameContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task BeforeExecuteAsync(GameContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task AfterExecuteAsync(GameContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task BeforeExitAsync(GameContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task AfterExitAsync(GameContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task OnErrorAsync(GameContext context, Exception exception, CancellationToken stoppingToken);
}
