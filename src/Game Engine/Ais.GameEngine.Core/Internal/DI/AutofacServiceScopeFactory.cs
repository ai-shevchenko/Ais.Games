using Ais.GameEngine.DependencyInjection.Abstractions;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.DI;

internal sealed class AutofacServiceScopeFactory : DependencyInjection.Abstractions.IServiceScopeFactory
{
    private readonly IContainer _container;

    public AutofacServiceScopeFactory(IContainer container)
    {
        _container = container;
    }

    public IServiceContainer CreateScope(
        string scopeName,
        Action<ScopeConfiguration>? configure = null)
    {
        var scopeConfig = new ScopeConfiguration { Name = scopeName };
        configure?.Invoke(scopeConfig);

        var scope = _container.BeginLifetimeScope(scopeName, builder =>
        {
            var services = new ServiceCollection();
            scopeConfig.ConfigureServices?.Invoke(services);
            builder.Populate(services);
        });

        return new AutofacServiceContainer(scope);
    }
}
