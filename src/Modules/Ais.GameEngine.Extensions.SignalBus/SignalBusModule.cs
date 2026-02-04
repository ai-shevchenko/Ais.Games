using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.SignalBus;

public sealed class SignalBusModule : GameEngineModule
{
    public override void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration)
    {
        gameServices.AddSingleton<ISignalBus, SignalBus>();
        gameServices.AddScoped<ISignalSubscriber>(sp => sp.GetRequiredService<ISignalBus>());
        gameServices.AddScoped<ISignalPublisher>(sp => sp.GetRequiredService<ISignalBus>());
    }
}
