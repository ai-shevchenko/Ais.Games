using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Components;
using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class RenderSystem : EcsSystem, IInitialize, IRender
{
    private readonly GameWindowSettings _settings;

    public RenderSystem(IOptions<GameWindowSettings> settings)
    {
        _settings = settings.Value;
    }

    public void Initialize()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        if (OperatingSystem.IsWindows())
        {
            Console.SetWindowSize(_settings.Width + 2, _settings.Height + 2);
        }
    }

    public void Render(float alpha)
    {
        var result = World.CreateQuery()
            .With<Position>()
            .With<Sprite>()
            .GetResult();

        Console.Clear();

        RenderBorders();

        foreach (var entity in result.Entities)
        {
            var position = entity.GetComponent<Position>(World);
            var sprite = entity.GetComponent<Sprite>(World);

            var originalBackground = Console.BackgroundColor;
            var originalForeground = Console.ForegroundColor;

            try
            {
                if (sprite.Symbol != '\0')
                {
                    Console.SetCursorPosition(position.X, position.Y);
                    Console.ForegroundColor = sprite.Color;
                    Console.Write(sprite.Symbol);
                }
            }
            finally
            {
                Console.BackgroundColor = originalBackground;
                Console.ForegroundColor = originalForeground;

                Console.SetCursorPosition(0, 0);
            }
        }
    }

    private void RenderBorders()
    {
        for (var idx = 0; idx < _settings.Height + 2; idx++)
        {
            Console.SetCursorPosition(0, idx);
            Console.Write('#');

            Console.SetCursorPosition(_settings.Width + 1, idx);
            Console.Write('#');
        }

        for (var idx = 0; idx < _settings.Width + 2; idx++)
        {
            Console.SetCursorPosition(idx, 0);
            Console.Write('#');

            Console.SetCursorPosition(idx, _settings.Height + 1);
            Console.Write('#');
        }
    }
}
