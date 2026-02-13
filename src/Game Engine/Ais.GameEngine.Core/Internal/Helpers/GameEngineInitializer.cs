using Ais.GameEngine.Core.Internal.GameLoop;
using Ais.GameEngine.Core.Internal.HooksSystem;
using Ais.GameEngine.Core.Internal.StateMachine;
using Ais.GameEngine.Core.Internal.TimeSystem;
using Ais.GameEngine.Core.Settings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.Helpers;

internal sealed class GameEngineInitializer
{
    public static void InitializeServices(
        IServiceCollection services,
        IConfigurationManager configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);
        services.AddOptions();

        services.AddStateMachine();
        services.AddTimeSystem();
        services.AddHooksSystem();
        services.AddGameLoopServices();

        var engineSettings = configuration.GetSection(nameof(GameEngineSettings));
        if (engineSettings.Exists())
        {
            services.Configure<GameEngineSettings>(engineSettings);
        }
        else
        {
            services.Configure<GameEngineSettings>(_ => { });
        }
    }

    public static IConfigurationManager InitializeConfiguration(GameEngineBuilderSettings settings)
    {
        var configuration = new ConfigurationManager();

        configuration.AddJsonFile("gamesettings.json", true, true);

        if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") is { Length: > 0 } env)
        {
            configuration.AddJsonFile($"gamesettings.{env}.json", true, true);
        }

        configuration
            .AddEnvironmentVariables()
            .AddCommandLine(settings.Args);

        return configuration;
    }
}
