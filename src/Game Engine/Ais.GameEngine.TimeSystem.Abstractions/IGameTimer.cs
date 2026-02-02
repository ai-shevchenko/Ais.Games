namespace Ais.GameEngine.TimeSystem.Abstractions;

/// <summary>
///     Игровой таймер для отслеживания времени игрового цикла
/// </summary>
public interface IGameTimer : IGameTimeSource, IGameTimerController, IDisposable
{
    /// <summary>
    ///     Глобальный игровой таймер
    /// </summary>
    ITimerController Parent { get; }

    /// <summary>
    ///     Создать таймер, для отслеживания времени внутри кадра
    /// </summary>
    /// <returns></returns>
    IFrameTimer CreateFrameTimer();
}
