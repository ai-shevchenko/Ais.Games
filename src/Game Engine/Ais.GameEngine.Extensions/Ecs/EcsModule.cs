using Ais.Commons.Modules;
using Ais.ECS;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Ecs;

internal sealed class EcsModule : IModule
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EcsSettings>(_ => { });
        services.AddScoped(sp =>
        {
            var builder = sp.GetRequiredService<IEcsWorldBuilder>();
            return builder.Build(sp);
        });
    }
}