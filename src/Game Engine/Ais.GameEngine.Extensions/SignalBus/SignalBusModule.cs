using Ais.Commons.SignalBus.Abstractions;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.SignalBus;

public sealed class SignalBusModule : GameEngineModule
{
    public override void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration)
    {
        gameServices.AddSingleton<ISignalBus, Commons.SignalBus.SignalBus>();
        gameServices.AddScoped<ISignalSubscriber>(sp => sp.GetRequiredService<ISignalBus>());
        gameServices.AddScoped<ISignalPublisher>(sp => sp.GetRequiredService<ISignalBus>());
    }

    public override void ConfigureGameLoop(GameLoopBuilderSettings settings)
    {
    }
}
