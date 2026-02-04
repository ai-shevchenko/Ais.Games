using Ais.GameEngine.StateMachine.Abstractions;
using Ais.GameEngine.TimeSystem.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class PauseState : GameLoopState
{
    private readonly ITimerController _time;
    private float _gameScale;

    public PauseState(ITimerController time)
    {
        _time = time;
    }

    public override Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        _gameScale = _time.Scale;
        _time.SetScale(0f);
        return base.EnterAsync(context, stoppingToken);
    }

    public override Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        _time.SetScale(_gameScale);
        return base.ExitAsync(context, stoppingToken);
    }
}
