namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Шина событий между игровыми циклами
/// </summary>
public interface IGameLoopEventBus
{
    /// <summary>
    ///     Подписаться на событие от игрового цикла.
    ///     События могут быть от разных циклов, но подписчик может указать имя цикла, от которого он хочет получать события.
    ///     Если имя цикла не указано, то подписчик будет получать события от всех циклов.
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <param name="loopName">Имя цикла</param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Подписка</returns>
    IDisposable Subscribe<TEvent>(string? loopName, Func<TEvent, CancellationToken, Task> handler)
        where TEvent : IGameLoopEvent;

    /// <summary>
    ///     Опубликовать событие от игрового цикла.
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <param name="evt">Событие</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IGameLoopEvent;

    /// <summary>
    ///     Опубликовать событие от игрового цикла.
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <param name="evt">Событие</param>
    /// <returns></returns>
    void Publish<TEvent>(TEvent evt)
        where TEvent : IGameLoopEvent;
}
