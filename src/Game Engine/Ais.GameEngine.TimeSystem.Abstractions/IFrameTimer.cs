using System;

namespace Ais.GameEngine.TimeSystem.Abstractions;

/// <summary>
/// Таймер, для измерения времени внутри кадра
/// </summary>
public interface IFrameTimer : IDisposable
{
    /// <summary>
    /// Игровой таймер
    /// </summary>
    IGameTimer GameTimer { get; }
    
    /// <summary>
    /// Целевая частота кадров
    /// </summary>
    float TargetFrameRate { get; }
    
    /// <summary>
    /// Частота кадров
    /// </summary>
    float FrameRate { get; }

    /// <summary>
    /// Время кадра
    /// </summary>
    float FrameTime { get; }

    /// <summary>
    /// Прошедшее время
    /// </summary>
    float Elapsed { get; }
    
    /// <summary>
    /// Получить время, необходимое до полного достижения целевой частоты кадров
    /// </summary>
    /// <returns>Время сна</returns>
    float GetSleepTime();
    
    /// <summary>
    /// Сбросить таймер
    /// </summary>
    void Restart();
}