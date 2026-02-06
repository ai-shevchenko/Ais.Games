
using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Signals;

using Microsoft.Extensions.Logging;

namespace Ais.Games.SnakeGame.Hooks;

internal sealed class LogSignalsHook : BaseHook, IInitialize, IDestroy
{
    private readonly ISignalBus _signalBus;
    private readonly ILogger<LogSignalsHook> _logger;
    private IDisposable? _sub;

    public LogSignalsHook(ISignalBus signalBus, ILogger<LogSignalsHook> logger)
    {
        _signalBus = signalBus;
        _logger = logger;
    }

    public void Initialize()
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }

        _sub = _signalBus.Subscribe<SnakeMoveSignal>(signal =>
        {
            var from = $"{signal.LastPosition.X}:{signal.LastPosition.Y}";
            var to = $"{signal.CurrentPosition.X}:{signal.CurrentPosition.Y}";

            _logger.LogDebug("The snake complete move ({@From} -> {@To})", from, to);
        });
    }

    public void OnDestroy()
    {
        _sub?.Dispose();
    }
}
