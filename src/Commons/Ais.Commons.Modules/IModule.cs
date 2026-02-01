using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.Commons.Modules;

public interface IModule
{
    void Configure(IServiceCollection services, IConfiguration configuration);
}