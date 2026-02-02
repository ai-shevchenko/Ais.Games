namespace Ais.GameEngine.TimeSystem.Abstractions;

/// <summary>
///     Игровой таймер для отслеживания времени игры
/// </summary>
public interface ITimerController : IGameTimerController, IDisposable
{
    /// <summary>
    ///     Масштаб времени
    /// </summary>
    float Scale { get; }

    /// <summary>
    ///     Создать дочерний игровой таймер
    /// </summary>
    /// <returns>Игровой таймер</returns>
    IGameTimer CreateChildTimer();

    /// <summary>
    ///     Создать дочерний именованный игровой таймер
    /// </summary>
    /// <param name="name">Наименование таймера</param>
    /// <returns>Игровой таймер</returns>
    IGameTimer CreateChildTimer(string name);

    /// <summary>
    ///     Получить дочерний именованный игровой таймер
    /// </summary>
    /// <param name="name">Наименование таймер</param>
    /// <returns>Игровой таймер</returns>
    IGameTimer GetChildTimer(string name);
}
