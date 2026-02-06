using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Systems;

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