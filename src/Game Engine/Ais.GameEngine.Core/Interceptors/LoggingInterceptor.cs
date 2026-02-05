using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Interceptors;

public sealed class LoggingInterceptor : GameLoopStateInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override Task BeforeEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetInterceptedState(context)?.GetType().Name;
            _logger.LogDebug("Entering state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public override Task AfterEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetInterceptedState(context)?.GetType().Name;
            _logger.LogDebug("Entered state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public override Task BeforeExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetInterceptedState(context)?.GetType().Name;
            _logger.LogDebug("Executing state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public override Task AfterExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetInterceptedState(context)?.GetType().Name;
            _logger.LogDebug("Executed state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public override Task BeforeExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetInterceptedState(context)?.GetType().Name;
            _logger.LogDebug("Exiting state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public override Task AfterExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetInterceptedState(context)?.GetType().Name;
            _logger.LogDebug("Exited state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }
}
