using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions;

public static class GameLoopExtensions
{
    public static GameLoopBuilderSettings AddModule<T>(GameLoopBuilderSettings gameLoopSettings)
        where T : class, IGameLoopModule, new()
    {
        var config = gameLoopSettings.GameServices
            .GetRequiredService<IConfiguration>();
        
        var module = new T();
        module.Configure(gameLoopSettings, config);

        return gameLoopSettings;
    }
}