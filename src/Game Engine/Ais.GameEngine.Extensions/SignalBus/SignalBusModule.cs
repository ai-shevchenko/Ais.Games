using Ais.Commons.Modules;
using Ais.Commons.SignalBus.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.SignalBus;

public sealed class SignalBusModule : IModule
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISignalBus, Commons.SignalBus.SignalBus>();
        services.AddScoped<ISignalSubscriber>(sp => sp.GetRequiredService<ISignalBus>());
        services.AddScoped<ISignalPublisher>(sp => sp.GetRequiredService<ISignalBus>());
    }
}