using Microsoft.Extensions.Configuration;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Настройки игрового цикла
/// </summary>
/// <param name="GameServices">Игровые сервисы</param>
/// <param name="GameConfiguration">Игровая конфигурация</param>
/// <param name="Hooks">Источник хуков</param>
public sealed record GameLoopBuilderSettings(
    IServiceProvider GameServices,
    IConfiguration GameConfiguration,
    IHooksSource Hooks);
