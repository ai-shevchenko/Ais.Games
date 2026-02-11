using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.Configuration;

internal sealed class ServicesConfigurator
{
    private readonly List<Action<GameEngineBuilderContext, IServiceCollection>> _configurators = [];

    public void AddConfigurator(Action<GameEngineBuilderContext, IServiceCollection> configure)
    {
        _configurators.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
    }

    public void ApplyAll(IServiceCollection services, GameEngineBuilderContext context)
    {
        foreach (var configurator in _configurators)
        {
            configurator(context, services);
        }
    }
}
