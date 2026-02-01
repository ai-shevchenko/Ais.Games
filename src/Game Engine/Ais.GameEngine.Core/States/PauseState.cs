using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.TimeSystem.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.States;

public sealed class PauseState : GameLoopState
{
    private readonly ILogger<PauseState> _logger;
    private readonly ITimerController _time;
    private float _gameScale;
    
    
    public PauseState(ITimerController time, ILogger<PauseState> logger)
    {
        _time = time;
        _logger = logger;
    }

    public override Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Entered pause for game loop {@LoopName}", context.LoopName);
        }

        _gameScale = _time.Scale;
        _time.SetScale(0f);
        return base.EnterAsync(context, stoppingToken);
    }

    public override Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Exit pause for game loop {@LoopName}", context.LoopName);
        }
        
        _time.SetScale(_gameScale);
        return base.ExitAsync(context, stoppingToken);
    }
}