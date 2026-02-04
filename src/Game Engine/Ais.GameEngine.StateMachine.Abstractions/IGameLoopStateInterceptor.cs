// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Перехватчик состояния игрового цикла
/// </summary>
public interface IGameLoopStateInterceptor
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task BeforeEnterAsync(GameLoopContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task AfterEnterAsync(GameLoopContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task BeforeExecuteAsync(GameLoopContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task AfterExecuteAsync(GameLoopContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task BeforeExitAsync(GameLoopContext context, CancellationToken stoppingToken);

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task AfterExitAsync(GameLoopContext context, CancellationToken stoppingToken);
}
