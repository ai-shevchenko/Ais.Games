using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Extensions.SignalBus;

public sealed class SignalBusModule : GameEngineModule
{
    public override void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration)
    {
        gameServices.TryAddSingleton<ISignalBus, SignalBus>();
        gameServices.TryAddSingleton<ISignalSubscriber>(sp => sp.GetRequiredService<ISignalBus>());
        gameServices.TryAddSingleton<ISignalPublisher>(sp => sp.GetRequiredService<ISignalBus>());
    }
}
