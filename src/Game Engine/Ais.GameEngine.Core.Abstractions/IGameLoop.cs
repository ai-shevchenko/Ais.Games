namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Игровой цикл с асинхронным управлением жизненным циклом.
///     Предоставляет полный контроль над запуском, остановкой, паузой и возобновлением цикла.
/// </summary>
public interface IGameLoop : IGameLoopController, IDisposable
{
    /// <summary>
    ///     Имя игрового цикла для идентификации.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Произойдёт когда цикл начинает выполняться.
    /// </summary>
    event EventHandler<GameLoopEventArgs>? Started;

    /// <summary>
    ///     Произойдёт когда цикл останавливается.
    /// </summary>
    event EventHandler<GameLoopEventArgs>? Stopped;

    /// <summary>
    ///     Произойдёт когда цикл приостанавливается.
    /// </summary>
    event EventHandler<GameLoopEventArgs>? Paused;

    /// <summary>
    ///     Произойдёт когда цикл возобновляется.
    /// </summary>
    event EventHandler<GameLoopEventArgs>? Resumed;

    /// <summary>
    ///     Произойдёт при ошибке в цикле.
    /// </summary>
    event EventHandler<GameLoopErrorEventArgs>? ErrorOccurred;
}
