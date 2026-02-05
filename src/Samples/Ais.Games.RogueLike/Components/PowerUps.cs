using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

internal enum PowerUpType
{
    SpeedBoost,
    DoubleScore
}

/// <summary>
/// Паверап, лежащий на поле.
/// </summary>
internal struct PowerUp : IComponent
{
    public PowerUpType Type;

    /// <summary>
    /// Время жизни паверапа в секундах.
    /// </summary>
    public float TimeToLiveSeconds;

    /// <summary>
    /// Сколько секунд он уже провёл на поле.
    /// </summary>
    public float ElapsedSeconds;
}

/// <summary>
/// Активный эффект паверапа на головном сегменте змейки.
/// </summary>
internal struct ActivePowerUpEffect : IComponent
{
    public PowerUpType Type;

    /// <summary>
    /// Оставшееся время действия эффекта в секундах.
    /// </summary>
    public float RemainingSeconds;
}

