using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Настройки игрового цикла
/// </summary>
public sealed record GameLoopBuilderSettings(
    IServiceCollection GameServices);
