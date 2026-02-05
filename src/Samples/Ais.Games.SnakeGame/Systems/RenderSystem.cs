using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Components;

using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class RenderSystem : EcsSystem, IInitialize, IRender
{
    private readonly GameWindowSettings _settings;

    private char[,]? _buffer;
    private ConsoleColor[,]? _colorBuffer;
    private char[,]? _prevBuffer;
    private ConsoleColor[,]? _prevColorBuffer;

    public RenderSystem(IOptions<GameWindowSettings> settings)
    {
        _settings = settings.Value;
    }

    private int BufferWidth => _settings.Width + 2;
    private int BufferHeight => _settings.Height + 3; // +1 строка под HUD

    public void Initialize()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        if (OperatingSystem.IsWindows())
        {
            Console.SetWindowSize(BufferWidth, BufferHeight);
        }

        _buffer = new char[BufferWidth, BufferHeight];
        _prevBuffer = new char[BufferWidth, BufferHeight];
        _colorBuffer = new ConsoleColor[BufferWidth, BufferHeight];
        _prevColorBuffer = new ConsoleColor[BufferWidth, BufferHeight];

        ClearBuffer(_buffer, _colorBuffer);
        ClearBuffer(_prevBuffer, _prevColorBuffer);
    }

    public void Render(float alpha)
    {
        if (_buffer is null || _prevBuffer is null || _colorBuffer is null || _prevColorBuffer is null)
        {
            return;
        }

        ClearBuffer(_buffer, _colorBuffer);

        DrawBorders();
        DrawEntities();
        DrawHud();

        FlushToConsole();
    }

    private void ClearBuffer(char[,] buffer, ConsoleColor[,] colors)
    {
        for (var y = 0; y < buffer.GetLength(1); y++)
        {
            for (var x = 0; x < buffer.GetLength(0); x++)
            {
                buffer[x, y] = ' ';
                colors[x, y] = ConsoleColor.Gray;
            }
        }
    }

    private void DrawBorders()
    {
        if (_buffer is null || _colorBuffer is null)
        {
            return;
        }

        for (var y = 0; y < _settings.Height + 2; y++)
        {
            _buffer[0, y] = '#';
            _buffer[_settings.Width + 1, y] = '#';
            _colorBuffer[0, y] = ConsoleColor.DarkGray;
            _colorBuffer[_settings.Width + 1, y] = ConsoleColor.DarkGray;
        }

        for (var x = 0; x < _settings.Width + 2; x++)
        {
            _buffer[x, 0] = '#';
            _buffer[x, _settings.Height + 1] = '#';
            _colorBuffer[x, 0] = ConsoleColor.DarkGray;
            _colorBuffer[x, _settings.Height + 1] = ConsoleColor.DarkGray;
        }
    }

    private void DrawEntities()
    {
        if (_buffer is null || _colorBuffer is null)
        {
            return;
        }

        var result = World.CreateQuery()
            .With<Position>()
            .With<Sprite>()
            .GetResult()
            .Entities;

        foreach (var entity in result)
        {
            var position = entity.GetComponent<Position>(World);
            var sprite = entity.GetComponent<Sprite>(World);

            if (sprite.Symbol == '\0')
            {
                continue;
            }

            if (position.X < 0 || position.X >= BufferWidth ||
                position.Y < 0 || position.Y >= BufferHeight - 1)
            {
                continue;
            }

            _buffer[position.X, position.Y] = sprite.Symbol;
            _colorBuffer[position.X, position.Y] = sprite.Color;
        }
    }

    private void DrawHud()
    {
        if (_buffer is null || _colorBuffer is null)
        {
            return;
        }

        var hudY = BufferHeight - 1;

        var scores = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        var value = 0;
        var fruits = 0;
        var powerUps = 0;

        if (scores.Length > 0)
        {
            var s = scores[0].GetComponent<Score>(World);
            value = s.Value;
            fruits = s.FruitsEaten;
            powerUps = s.PowerUpsCollected;
        }

        var text =
            $"Score: {value}  Fruits: {fruits}  PowerUps: {powerUps}  Controls: Arrows/WASD - move, Esc - exit";

        for (var x = 0; x < BufferWidth && x < text.Length; x++)
        {
            _buffer[x, hudY] = text[x];
            _colorBuffer[x, hudY] = ConsoleColor.White;
        }
    }

    private void FlushToConsole()
    {
        if (_buffer is null || _prevBuffer is null || _colorBuffer is null || _prevColorBuffer is null)
        {
            return;
        }

        for (var y = 0; y < BufferHeight; y++)
        {
            for (var x = 0; x < BufferWidth; x++)
            {
                var ch = _buffer[x, y];
                var color = _colorBuffer[x, y];

                if (ch == _prevBuffer[x, y] && color == _prevColorBuffer[x, y])
                {
                    continue;
                }

                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = color;
                Console.Write(ch);

                _prevBuffer[x, y] = ch;
                _prevColorBuffer[x, y] = color;
            }
        }

        Console.SetCursorPosition(0, BufferHeight - 1);
    }
}
