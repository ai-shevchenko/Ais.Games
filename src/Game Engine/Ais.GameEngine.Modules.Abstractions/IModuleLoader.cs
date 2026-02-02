using System.Reflection;

namespace Ais.GameEngine.Modules.Abstractions;

/// <summary>
/// Загрузчик модулей
/// </summary>
public interface IModuleLoader
{
    /// <summary>
    /// Загруженные модули
    /// </summary>
    IReadOnlyList<Type> LoadModules { get; }
    
    /// <summary>
    /// Загрузить модуль из сборки
    /// </summary>
    /// <param name="assembly">Сборка</param>
    void LoadAssembly(Assembly assembly);
    
    /// <summary>
    /// Загрузить модуль из сборки
    /// </summary>
    /// <param name="assemblyName">Наименование сборки</param>
    void LoadAssembly(string assemblyName);
    
    /// <summary>
    /// Загрузить модуль из DLL
    /// </summary>
    /// <param name="path">Путь к DLL файлу</param>
    void LoadDll(string path);
    
    /// <summary>
    /// Загрузить модули из указанной директории
    /// </summary>
    /// <param name="path"></param>
    void LoadFromDirectory(string path);
    
    /// <summary>
    /// Получить список загруженных модулей
    /// </summary>
    /// <returns>Список модулей</returns>
    IReadOnlyList<GameEngineModule> GetLoadedModules();
}