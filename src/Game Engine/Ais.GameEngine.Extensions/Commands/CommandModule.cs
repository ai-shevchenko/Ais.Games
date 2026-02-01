using Ais.Commons.Commands;
using Ais.Commons.Commands.Abstractions;
using Ais.Commons.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Extensions.Commands;

public sealed class CommandModule : IModule
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<ICommandQueue, CommandQueue>();
        services.TryAddScoped(sp => (ICommandExecutor) sp.GetRequiredService<ICommandQueue>());
    }
}