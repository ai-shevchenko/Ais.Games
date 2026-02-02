using System.Reflection;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopFactorySettings
{
    /// <summary>
    /// Список сборок, содержащих доп модули
    /// </summary>
    public Dictionary<string, Assembly[]> AssemblyModules { get; init; } = [];
    
    /// <summary>
    /// Список DLL содержащих доп модули
    /// </summary>
    public Dictionary<string, string[]> DllModules { get; init; } = [];
}