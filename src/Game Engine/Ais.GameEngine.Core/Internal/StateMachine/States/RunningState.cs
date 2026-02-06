using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.GameEngine.TimeSystem.Abstractions;

namespace Ais.GameEngine.Core.Internal.StateMachine.States;

internal sealed class RunningState : GameStateBase, IDisposable
{
    private readonly IFrameTimer _frameTimer;
    private readonly IGameTimer _gameTimer;
    private readonly IHooksProvider _hooksProvider;

    public RunningState(ITimerController timer, IHooksProvider hooksProvider)
    {
        _hooksProvider = hooksProvider;
        _gameTimer = timer.CreateChildTimer();
        _frameTimer = _gameTimer.CreateFrameTimer();
    }

    public void Dispose()
    {
        _frameTimer.Dispose();
        _gameTimer.Dispose();
    }

    public override Task EnterAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        _gameTimer.Restart();
        return Task.CompletedTask;
    }

    public override async Task ExecuteAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        _frameTimer.Restart();

        var deltaTime = _gameTimer.DeltaTime;

        while (_gameTimer.ShouldFixedUpdate)
        {
            await FixedUpdateAsync(_gameTimer.FixedDeltaTime, stoppingToken);
        }

        await UpdateAsync(deltaTime, stoppingToken);

        await LateUpdateAsync(deltaTime, stoppingToken);

        var alpha = _gameTimer.InterpolationFactor;
        await RenderAsync(alpha, stoppingToken);

        var sleep = _frameTimer.GetSleepTime() * 1_000;
        await Task.Delay((int)sleep, stoppingToken);
    }

    public override Task ExitAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        _gameTimer.Stop();
        return Task.CompletedTask;
    }

    private async Task FixedUpdateAsync(float fixedDeltaTime,
        CancellationToken stoppingToken)
    {
        foreach (var hook in _hooksProvider.GetHooks<IFixedUpdate>(true))
        {
            hook.FixedUpdate(fixedDeltaTime);
        }

        var asyncHooks = _hooksProvider
            .GetHooks<IAsyncFixedUpdate>(true)
            .Select(hook => hook.FixedUpdateAsync(fixedDeltaTime, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    private async Task UpdateAsync(float deltaTime, CancellationToken stoppingToken)
    {
        foreach (var hook in _hooksProvider.GetHooks<IUpdate>(true))
        {
            hook.Update(deltaTime);
        }

        var asyncHooks = _hooksProvider
            .GetHooks<IAsyncUpdate>(true)
            .Select(hook => hook.UpdateAsync(deltaTime, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    private async Task LateUpdateAsync(float deltaTime, CancellationToken stoppingToken)
    {
        foreach (var hook in _hooksProvider.GetHooks<ILateUpdate>(true))
        {
            hook.LateUpdate(deltaTime);
        }

        var asyncHooks = _hooksProvider
            .GetHooks<IAsyncLateUpdate>(true)
            .Select(hook => hook.LateUpdateAsync(deltaTime, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    private async Task RenderAsync(float alpha, CancellationToken stoppingToken)
    {
        foreach (var hook in _hooksProvider.GetHooks<IRender>(true))
        {
            hook.Render(alpha);
        }

        var asyncHooks = _hooksProvider
            .GetHooks<IAsyncRender>(true)
            .Select(hook => hook.RenderAsync(alpha, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }
}
