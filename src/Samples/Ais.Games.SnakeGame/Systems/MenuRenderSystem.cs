using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class MenuRenderSystem : EcsSystem, IRender
{
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
}

internal sealed class MenuInputSystem : EcsSystem, IInitialize, IAsyncUpdate
{
    private readonly GameSession _session;
    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    public MenuInputSystem(GameSession session)
    {
        _session = session;
    }

    public async Task UpdateAsync(float deltaTime, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public void Initialize()
    {
        _cts = new CancellationTokenSource();
        _loopTask = Task.Run(() => LoopAsync(_cts.Token), _cts.Token);
    }

    public override void Shutdown()
    {
        _cts?.Cancel();
        if (_loopTask is not null)
        {
            Task.WhenAny(_loopTask).Wait();
        }
    }

    private async Task LoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
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

            await Task.Delay(10, token);
        }
    }
}

internal sealed class GameOverRenderSystem : EcsSystem, IRender
{
    private readonly GameSession _session;

    public GameOverRenderSystem(GameSession session)
    {
        _session = session;
    }

    public void Render(float alpha)
    {
        Console.Clear();
        Console.CursorVisible = false;

        var message = _session.State switch
        {
            GameState.Won => "YOU WON!",
            GameState.Lost => "GAME OVER",
            GameState.Exit => "GOOD BYE",
            _ => "GAME OVER"
        };

        Console.SetCursorPosition(5, 5);
        Console.WriteLine(message);

        Console.SetCursorPosition(5, 7);
        Console.WriteLine("Press ENTER to go back to menu or ESC to exit.");
    }
}
