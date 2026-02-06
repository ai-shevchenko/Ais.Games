using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Hooks;

internal sealed class MainMenuHook : BaseHook, IInitialize, IRender, IDestroy
{
    private readonly TimeSpan _inputDelay = TimeSpan.FromMilliseconds(10);
    private readonly GameSession _session;

    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    public MainMenuHook(GameSession session)
    {
        _session = session;
    }

    public void Initialize()
    {
        _cts = new CancellationTokenSource();
        _loopTask = Task.Run(() => ReadInputAsync(_cts.Token), _cts.Token);
    }

    public void Render(float alpha)
    {
        Console.Clear();
        Console.CursorVisible = false;

        var options = new[]
        {
            "=== SNAKE GAME ===",
            "", "1. Start game",
            "2. Exit", "",
            "Use keys 1-2 to choose."
        };

        var top = 2;
        for (var i = 0; i < options.Length; i++)
        {
            Console.SetCursorPosition(2, top + i);
            Console.WriteLine(options[i]);
        }
    }

    public void OnDestroy()
    {
        _cts?.Cancel();
        if (_loopTask is not null)
        {
            Task.WhenAny(_loopTask).Wait();
        }
    }

    private async Task ReadInputAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    _session.SetResult(GameState.Start);
                    return;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                case ConsoleKey.Escape:
                    _session.SetResult(GameState.Exit);
                    return;
            }

            await Task.Delay(_inputDelay, token);
        }
    }
}
