namespace Ais.GameEngine.TimeSystem.Abstractions;

public interface IGameTimeSource
{
    /// <summary>
    ///     Определяет требование к выполнению фиксированного обновления
    /// </summary>
    bool ShouldFixedUpdate { get; }

    /// <summary>
    ///     Фактор интерполяции для фиксированного обновления (используется для интерполяции между фиксированными шагами)
    /// </summary>
    float InterpolationFactor { get; }

    /// <summary>
    ///     Фиксированный шаг времени
    /// </summary>
    float FixedDeltaTime { get; }

    /// <summary>
    ///     Время, прошедшее между кадрами
    /// </summary>
    float DeltaTime { get; }

    /// <summary>
    ///     Общее время, прошедшее с момента старта таймера
    /// </summary>
    float TotalTime { get; }

    /// <summary>
    ///     Текущий масштаб времени
    /// </summary>
    float Scale { get; }
}
