using Ais.GameEngine.Core.Internal.StateMachine.States;
using Ais.GameEngine.Modules.Abstractions.Extensions;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.StateMachine;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStateMachine(this IServiceCollection services)
    {
        services
            .AddScoped<IGameStateProvider, GameStateProvider>()
            .AddScoped<IGameContextAccessor, GameContextAccessor>()
            .AddScoped<IGameStateMachine, GameStateMachine>()
            .AddScoped<IGameStateExecutor, GameStateExecutor>();

        services
            .AddScopedState<InitializeState>()
            .AddScopedState<PauseState>()
            .AddScopedState<RunningState>()
            .AddScopedState<StoppingState>();

        return services;
    }
}
