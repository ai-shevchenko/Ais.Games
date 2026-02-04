using Ais.GameEngine.Modules.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Ais.GameEngine.Core.Modules;

public class ConfigurationModuleEnricher : IModuleEnricher
{
    private const string SectionName = "GameEngineModules";
    private readonly IConfiguration _configuration;

    public ConfigurationModuleEnricher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Enrich(IKeyedModuleLoader loader)
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
                    loader.LoadFromDirectory(configList.Key, moduleValue);
                    continue;
                }

                if (File.Exists(moduleValue) && Path.GetExtension(moduleValue) == ".dll")
                {
                    loader.LoadDll(configList.Key, moduleValue);
                    continue;
                }

                loader.LoadAssembly(configList.Key, moduleValue);
            }
        }
    }
}
