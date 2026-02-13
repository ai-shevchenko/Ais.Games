using Ais.GameEngine.StateMachine.Abstractions;
using Ais.GameEngine.TimeSystem.Abstractions;

namespace Ais.GameEngine.Core.Internal.StateMachine.States;

internal sealed class PauseState : GameStateBase
{
    private readonly IGameContextAccessor _context;
    private readonly ITimerController _timer;
    private float _gameScale;

    public PauseState(ITimerController timer, IGameContextAccessor context)
    {
        _timer = timer;
        _context = context;
    }

    public override Task EnterAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        _gameScale = _timer.Scale;
        _timer.GetChildTimer(_context.CurrentContext!.LoopName).SetScale(0);
        return base.EnterAsync(context, stoppingToken);
    }

    public override Task ExitAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        _timer.GetChildTimer(_context.CurrentContext!.LoopName).SetScale(_gameScale);
        return base.ExitAsync(context, stoppingToken);
    }
}
