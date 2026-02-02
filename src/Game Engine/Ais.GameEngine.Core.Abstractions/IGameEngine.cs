namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Игровой движок
/// </summary>
public interface IGameEngine : IGameLoopManager, IDisposable
{
    /// <summary>
    /// Запустить игру
    /// </summary>
    /// <param name="stoppingToken">Токен остановки</param>
    void Start(CancellationToken stoppingToken = default);
    
    /// <summary>
    /// Остановить игру
    /// </summary>
    void Stop();
}