using System.Reflection;

namespace Ais.GameEngine.Modules.Abstractions;

public interface IKeyedModuleLoader : IModuleLoader
{
    /// <summary>
    ///     Загрузить модуль из сборки
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="assembly">Сборка</param>
    void LoadAssembly(string key, Assembly assembly);

    /// <summary>
    ///     Загрузить модуль из сборки
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="assemblyName">Наименование сборки</param>
    void LoadAssembly(string key, string assemblyName);

    /// <summary>
    ///     Загрузить модуль из DLL
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="path">Путь к DLL файлу</param>
    void LoadDll(string key, string path);

    /// <summary>
    ///     Загрузить модули из указанной директории
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="path">Директория</param>
    void LoadFromDirectory(string key, string path);

    /// <summary>
    ///     Получить список загруженных модулей
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <returns>Список модулей</returns>
    IReadOnlyList<GameEngineModule> GetLoadedModules(string key);
}
