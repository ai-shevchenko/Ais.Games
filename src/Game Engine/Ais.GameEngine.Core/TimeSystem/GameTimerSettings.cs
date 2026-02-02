using System.ComponentModel.DataAnnotations;


namespace Ais.GameEngine.Core.TimeSystem;

/// <summary>
/// Настройки игрового таймера
/// </summary>
public class GameTimerSettings
{
    /// <summary>
    /// Фиксированный шаг времени 
    /// </summary>
    [Range(0, 1f)]
    public float FixedDeltaTime { get; set; } = 0.016f;

    /// <summary>
    /// Максимальный размер кадра
    /// </summary>
    [Range(0, 1f)]
    public float MaxFrameTime { get; set; } = 0.25f;

    /// <summary>
    /// Целевая частота кадров
    /// </summary>
    [Range(24, float.MaxValue)]
    public float TargetFrameRate { get; set; } = 60;

    /// <summary>
    /// Максимальное кол-во фиксированных обновлений за кадр
    /// </summary>
    [Range(1, int.MaxValue)]
    public int MaxFixedUpdateIteration { get; set; } = 10;
}