using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

internal struct SnakeSegment : IComponent
{
    /// <summary>
    /// Признак головного сегмента.
    /// </summary>
    public bool IsHead;

    /// <summary>
    /// Порядковый номер сегмента (0 - голова, далее по цепочке).
    /// Используется системой перемещения, чтобы двигать сегменты
    /// в правильном порядке.
    /// </summary>
    public int Order;
}
