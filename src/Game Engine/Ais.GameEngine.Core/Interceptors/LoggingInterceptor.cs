using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Interceptors;

public sealed class LoggingInterceptor : IGameLoopStateInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public Task BeforeEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetType().Name;
            _logger.LogDebug("Entering state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public Task AfterEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetType().Name;
            _logger.LogDebug("Entered state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public Task BeforeExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetType().Name;
            _logger.LogDebug("Executing state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public Task AfterExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetType().Name;
            _logger.LogDebug("Executed state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public Task BeforeExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetType().Name;
            _logger.LogDebug("Exiting state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }

    public Task AfterExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var name = GetType().Name;
            _logger.LogDebug("Exited state {LoopName}::{StateName}", context.LoopName, name);
        }

        return Task.CompletedTask;
    }
}
