using Ais.GameEngine.Core.Settings;
using Ais.GameEngine.Modules.Abstractions;

namespace Ais.GameEngine.Core;

/// <summary>
///     Настройки фабрики игрового движка
/// </summary>
public sealed class GameEngineBuilderSettings
{
    /// <summary>
    ///     Аргументы командной строки
    /// </summary>
    public string[] Args { get; init; } = [];

    /// <summary>
    ///     Настройки игрового движка
    /// </summary>
    public GameEngineSettings? GameEngineSettings { get; init; }

    /// <summary>
    ///     Загрузчик модулей
    /// </summary>
    public IModuleLoader? ModuleLoader { get; init; }
}
