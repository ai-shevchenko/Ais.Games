using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

internal struct SnakeSegment : IComponent
{
    public bool IsHead;
}