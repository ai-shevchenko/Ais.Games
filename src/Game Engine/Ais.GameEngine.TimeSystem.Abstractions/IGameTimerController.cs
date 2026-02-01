namespace Ais.GameEngine.TimeSystem.Abstractions;

/// <summary>
/// Управление временем
/// </summary>
public interface IGameTimerController
{
    /// <summary>
    /// Состояние таймера вкл/выкл
    /// </summary>
    bool IsRunning { get; }
    
    /// <summary>
    /// Запустить таймер
    /// </summary>
    void Start();
    
    /// <summary>
    /// Остановить таймер
    /// </summary>
    void Stop();
    
    /// <summary>
    /// Сбросить таймер
    /// </summary>
    void Restart();
    
    /// <summary>
    /// Установить масштаб времени
    /// </summary>
    /// <param name="scale">Масштаб времени</param>
    void SetScale(float scale);
    
    /// <summary>
    /// Сбросить масштаб времени
    /// </summary>
    void ResetScale();
}