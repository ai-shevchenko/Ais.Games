namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Настройки игрового цикла
/// </summary>
/// <param name="GameServices">Игровые сервисы</param>
/// <param name="Hooks">Источник хуков</param>
/// <param name="States">Источник состояний</param>
public sealed record GameLoopBuilderSettings(
    IServiceProvider GameServices,
    IHooksSource Hooks,
    IGameLoopStateSource States);