using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Internal.Helpers;

internal sealed class LoggingConfigurator
{
    private Action<GameEngineBuilderContext, ILoggingBuilder>? _configure;

    public void Configure(Action<GameEngineBuilderContext, ILoggingBuilder> configure)
    {
        _configure = configure;
    }

    public void Apply(IServiceCollection services, GameEngineBuilderContext context)
    {
        services.AddLogging(builder =>
        {
            _configure?.Invoke(context, builder);
        });
    }
}
