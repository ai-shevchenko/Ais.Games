using Ais.ECS;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.Modules.Abstractions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Extensions.Ecs;

public sealed class EcsModule : GameEngineModule
{
    public override void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration)
    {
        gameServices.Configure<EcsSettings>(_ => { });

        gameServices.TryAddSingleton<IEcsWorldBuilder>(EcsWorldBuilder.Instance);
        gameServices.AddScoped(sp =>
        {
            var builder = sp.GetRequiredService<IEcsWorldBuilder>();
            return builder.Build(sp);
        });

        gameServices.AddHook<EcsSystemHandlerHook>();
        gameServices.AddHook<EcsSystemAsyncHandlerHook>();
    }
}
