namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Исполнитель состояний
/// </summary>
public interface IGameStateExecutor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task EnterAsync(IGameState state, CancellationToken cancellationToken = default);


    /// <summary>
    ///     Исполнить состояние
    /// </summary>
    /// <param name="state">Состояние</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task ExecuteAsync(IGameState state, CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExitAsync(IGameState state, CancellationToken cancellationToken = default);
}
