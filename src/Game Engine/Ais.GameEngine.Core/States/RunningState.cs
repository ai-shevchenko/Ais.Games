using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.TimeSystem.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.States;

public sealed class RunningState : GameLoopState, IDisposable
{
    private readonly IGameTimer _gameTimer;
    private readonly IFrameTimer _frameTimer;
    private readonly ILogger<RunningState> _logger;
    
    public RunningState(ITimerController timer, ILogger<RunningState> logger)
    {
        _logger = logger;
        _gameTimer = timer.CreateChildTimer();
        _frameTimer = _gameTimer.CreateFrameTimer();
    }

    public override Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Running game loop {@LoopName}",  context.LoopName);
        }
        
        _gameTimer.Restart();
        return Task.CompletedTask;
    }

    public override async Task ExecuteAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        _frameTimer.Restart();

        var deltaTime = _gameTimer.DeltaTime;

        while (_gameTimer.ShouldFixedUpdate)
        {
            await FixedUpdateAsync(context, _gameTimer.FixedDeltaTime, stoppingToken);
        }

        await UpdateAsync(context, deltaTime, stoppingToken);

        await LateUpdateAsync(context, deltaTime, stoppingToken);

        var alpha = _gameTimer.InterpolationFactor;
        await RenderAsync(context, alpha, stoppingToken);

        var sleep = _frameTimer.GetSleepTime() * 1_000;
        await Task.Delay((int)sleep, stoppingToken);
    }

    public override Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        _gameTimer.Stop();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _frameTimer.Dispose();
        _gameTimer.Dispose();
    }

    private static async Task FixedUpdateAsync(GameLoopContext context, float fixedDeltaTime, CancellationToken stoppingToken)
    {
        foreach (var hook in context.Hooks.GetHooks<IFixedUpdate>(enabledOnly: true))
        {
            hook.FixedUpdate(fixedDeltaTime);
        }

        var asyncHooks = context.Hooks
            .GetHooks<IAsyncFixedUpdate>(enabledOnly: true)
            .Select(hook => hook.FixedUpdateAsync(fixedDeltaTime, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    private static async Task UpdateAsync(GameLoopContext context, float deltaTime, CancellationToken stoppingToken)
    {
        foreach (var hook in context.Hooks.GetHooks<IUpdate>(enabledOnly: true))
        {
            hook.Update(deltaTime);
        }

        var asyncHooks = context.Hooks
            .GetHooks<IAsyncUpdate>(enabledOnly: true)
            .Select(hook => hook.UpdateAsync(deltaTime, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    private static async Task LateUpdateAsync(GameLoopContext context, float deltaTime, CancellationToken stoppingToken)
    {
        foreach (var hook in context.Hooks.GetHooks<ILateUpdate>(enabledOnly: true))
        {
            hook.LateUpdate(deltaTime);
        }

        var asyncHooks = context.Hooks
            .GetHooks<IAsyncLateUpdate>(enabledOnly: true)
            .Select(hook => hook.LateUpdateAsync(deltaTime, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    private static async Task RenderAsync(GameLoopContext context, float alpha, CancellationToken stoppingToken)
    {
        foreach (var hook in context.Hooks.GetHooks<IRender>(enabledOnly: true))
        {
            hook.Render(alpha);
        }

        var asyncHooks = context.Hooks
            .GetHooks<IAsyncRender>(enabledOnly: true)
            .Select(hook => hook.RenderAsync(alpha, stoppingToken));

        await Task.WhenAll(asyncHooks);
    }
}