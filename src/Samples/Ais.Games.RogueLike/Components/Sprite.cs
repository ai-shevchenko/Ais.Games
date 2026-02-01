using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

internal struct Sprite : IComponent
{
    public char Symbol;
    public ConsoleColor Color;
}