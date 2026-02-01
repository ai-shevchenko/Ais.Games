namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Игровой цикл
/// </summary>
public interface IGameLoop : IDisposable
{    
    /// <summary>
    /// Запустить игровой цикл
    /// </summary>
    /// <param name="stoppingToken">Токен остановки</param>
    void Start(CancellationToken stoppingToken = default);
    
    /// <summary>
    /// Остановить игровой цикл
    /// </summary>
    void Stop();
    
    /// <summary>
    /// Поставить игровой цикл на паузу
    /// </summary>
    void Pause();
    
    /// <summary>
    /// Продолжить игровой цикл
    /// </summary>
    void Resume();
}