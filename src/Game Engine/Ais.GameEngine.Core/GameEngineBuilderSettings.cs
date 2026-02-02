using System.Reflection;

using Ais.GameEngine.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

/// <summary>
/// Настройки фабрики игрового движка
/// </summary>
public sealed class GameEngineBuilderSettings
{
    /// <summary>
    /// Аргументы командной строки
    /// </summary>
    public string[] Args { get; init; } = [];

    /// <summary>
    /// Список сборок, содержащих доп модули
    /// </summary>
    public Dictionary<string, Assembly[]> AssemblyModules { get; init; } = [];
    
    /// <summary>
    /// Список DLL содержащих доп модули
    /// </summary>
    public Dictionary<string, string[]> DllModules { get; init; } = [];
    
    /// <summary>
    /// Настройки поставщика сервисов
    /// </summary>
    public ServiceProviderOptions? ServiceProviderOptions { get; init; }

    /// <summary>
    /// Настройки игрового движка
    /// </summary>
    public GameEngineSettings? GameEngineSettings { get; init; }
}
