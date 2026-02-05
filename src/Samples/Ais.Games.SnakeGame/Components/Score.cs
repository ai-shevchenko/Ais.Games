using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

/// <summary>
///     Статистика текущей игровой сессии.
///     Хранится на единственной "сессионной" сущности.
/// </summary>
internal struct Score : IComponent
{
    /// <summary>
    ///     Текущий счёт.
    /// </summary>
    public int Value;

    /// <summary>
    ///     Количество съеденных фруктов.
    /// </summary>
    public int FruitsEaten;

    /// <summary>
    ///     Количество собранных паверапов.
    /// </summary>
    public int PowerUpsCollected;

    /// <summary>
    ///     Множитель очков (1 по умолчанию, может временно увеличиваться паверапом).
    /// </summary>
    public int ScoreMultiplier;
}
