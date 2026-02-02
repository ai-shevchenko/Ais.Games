using Ais.Commons.Commands;
using Ais.Commons.Commands.Abstractions;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Extensions.Commands;

public sealed class CommandsModule : GameEngineModule
{
    public override void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration)
    {
        gameServices.TryAddScoped<ICommandQueue, CommandQueue>();
        gameServices.TryAddScoped<ICommandExecutor>(sp => sp.GetRequiredService<ICommandQueue>());
    }

    public override void ConfigureGameLoop(GameLoopBuilderSettings settings)
    {
    }
}
