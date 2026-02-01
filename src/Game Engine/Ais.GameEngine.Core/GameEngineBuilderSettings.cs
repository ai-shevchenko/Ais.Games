using Ais.GameEngine.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

public class GameEngineBuilderSettings
{
    public string[] Args { get; init; } = [];

    public ServiceProviderOptions? ServiceProviderOptions { get; init; }

    public GameEngineSettings? GameEngineSettings { get; init; }
}
