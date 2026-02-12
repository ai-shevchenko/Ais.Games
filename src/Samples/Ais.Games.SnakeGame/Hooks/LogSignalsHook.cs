using Ais.GameEngine.Hooks.Abstractions;

using Microsoft.Extensions.Logging;

namespace Ais.Games.SnakeGame.Hooks;

internal sealed class LogSignalsHook : BaseHook, IInitialize, IDestroy
{
    private readonly ILogger<LogSignalsHook> _logger;
    private IDisposable? _sub;

    public LogSignalsHook(ILogger<LogSignalsHook> logger)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }
    }

    public void OnDestroy()
    {
        _sub?.Dispose();
    }
}
