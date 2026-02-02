using System.Reflection;

using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.Configuration;

namespace Ais.GameEngine.Core;

public class ModuleEnricher : IModuleEnricher
{
    private const string SectionName = "GameEngineModules";
    
    private readonly IKeyedModuleLoader _loader;
    private readonly IConfiguration _configuration;

    public ModuleEnricher(IConfiguration configuration, IKeyedModuleLoader loader)
    {
        _configuration = configuration;
        _loader = loader;
    }

    public void Enrich()
    {
        var section = _configuration.GetSection(SectionName);
        if (!section.Exists())
        {
            return;
        }

        foreach (var configList in section.GetChildren())
        {
            foreach (var configSection in configList.GetChildren())
            {   
                var moduleValue = configSection.Get<string>();
                if (string.IsNullOrWhiteSpace(moduleValue))
                {
                    continue;
                }

                if (Directory.Exists(moduleValue))
                {
                    _loader.LoadFromDirectory(configList.Key, moduleValue);
                    continue;
                }
            
                if (File.Exists(moduleValue) && Path.GetExtension(moduleValue) == ".dll")
                {
                    _loader.LoadDll(configList.Key, moduleValue);
                    continue;
                }
                
                _loader.LoadAssembly(configList.Key, moduleValue);
            }
        }
    }
}