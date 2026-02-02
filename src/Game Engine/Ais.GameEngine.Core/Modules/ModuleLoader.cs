using System.Reflection;

using Ais.GameEngine.Modules.Abstractions;

namespace Ais.GameEngine.Core.Modules;

public sealed class ModuleLoader : IKeyedModuleLoader
{
    private const string DefaultName = "Default";
    
    private readonly List<Type> _types = [];
    private readonly Dictionary<string, List<GameEngineModule>> _modules = [];

    public IReadOnlyList<Type> LoadModules => _types.AsReadOnly();

    public void LoadAssembly(Assembly assembly)
    {
        LoadAssembly(DefaultName, assembly);
    }

    public void LoadAssembly(string assemblyName)
    {
        LoadAssembly(DefaultName, assemblyName);
    }

    public void LoadDll(string path)
    {
        LoadDll(DefaultName, path);
    }

    public void LoadFromDirectory(string path)
    {
        LoadFromDirectory(DefaultName, path);
    }

    public IReadOnlyList<GameEngineModule> GetLoadedModules()
    {
        return _modules.Values
            .SelectMany(m => m)
            .ToList()
            .AsReadOnly();
    }

    public void LoadAssembly(string key, Assembly assembly)
    {
        var moduleTypes = assembly.GetTypes()
            .Where(t => typeof(GameEngineModule).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in moduleTypes)
        {
            if (_types.Contains(type))
            {
                continue;
            }
            
            var module = (GameEngineModule)Activator.CreateInstance(type)!;
            if (!_modules.TryGetValue(key, out var modules))
            {
                modules = [];
                _modules.Add(key, modules);
            }
            modules.Add(module);
            
            _types.Add(type);
        }
    }

    public void LoadAssembly(string key, string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        LoadAssembly(key, assembly);
    }

    public void LoadDll(string key, string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file {path} was not found.");
        }

        if (Path.GetExtension(path) != ".dll")
        {
            throw new FileLoadException($"The file {path} was not DLL.");
        }
        
        var assembly = Assembly.LoadFrom(path);
        LoadAssembly(key, assembly);
    }

    public void LoadFromDirectory(string key, string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }
        
        var files = Directory.GetFiles(path, "*.dll");
        foreach (var file in files)
        {
            LoadDll(key, file);
        }
    }

    public IReadOnlyList<GameEngineModule> GetLoadedModules(string key)
    {
        if (_modules.TryGetValue(key, out var modules))
        {
            return modules.AsReadOnly();
        }

        return [];
    }
}