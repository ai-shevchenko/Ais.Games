using Ais.ECS;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.Modules.Abstractions.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Ecs;

public sealed class EcsModule : GameEngineModule
{
    public override void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration)
    {
        gameServices.Configure<EcsSettings>(_ => { });
        gameServices.AddScoped(sp => sp.GetRequiredService<IEcsWorldBuilder>().Build(sp));

        gameServices.AddScopedHook<EcsSystemHandlerHook>();
        gameServices.AddScopedHook<EcsSystemAsyncHandlerHook>();
    }
}
