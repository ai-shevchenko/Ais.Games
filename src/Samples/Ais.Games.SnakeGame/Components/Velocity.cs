using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

internal struct Velocity : IComponent
{
    public int DirectionX;
    public int DirectoinY;
}
