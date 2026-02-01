using Ais.Commons.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Commands;

public static class CommandExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddModule<CommandModule>(configuration);
    }
}