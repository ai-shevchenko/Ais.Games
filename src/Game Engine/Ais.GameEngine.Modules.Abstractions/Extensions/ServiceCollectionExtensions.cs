using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Modules.Abstractions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonState<TState>(this IServiceCollection services)
        where TState : class, IGameLoopState
    {
        return services.AddSelfService<IGameLoopState, TState>(ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddScopedState<TState>(this IServiceCollection services)
        where TState : class, IGameLoopState
    {
        return services.AddSelfService<IGameLoopState, TState>(ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddTransientState<TState>(this IServiceCollection services)
        where TState : class, IGameLoopState
    {
        return services.AddSelfService<IGameLoopState, TState>(ServiceLifetime.Transient);
    }

    /// <summary>
    ///     Добавить перехватчик состояний
    /// </summary>
    /// <param name="services">Список сервисов</param>
    /// <param name="lifetime">Жизненный цикл состояния</param>
    /// <typeparam name="TInterceptor">Тип перехватчика</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddStateInterceptor<TInterceptor>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TInterceptor : class, IGameLoopStateInterceptor
    {
        return services.AddSelfService<IGameLoopStateInterceptor, TInterceptor>(lifetime);
    }

    public static IServiceCollection AddSingletonHook<THook>(this IServiceCollection services)
        where THook : class, IHook
    {
        return services.AddSelfService<IHook, THook>(ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddScopedHook<THook>(this IServiceCollection services)
        where THook : class, IHook
    {
        return services.AddSelfService<IHook, THook>(ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddTransientHook<THook>(this IServiceCollection services)
        where THook : class, IHook
    {
        return services.AddSelfService<IHook, THook>(ServiceLifetime.Transient);
    }

    public static IServiceCollection AddSelfService<TService, TImplementation>(
        this IServiceCollection services, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAdd(
            new ServiceDescriptor(
                typeof(TImplementation),
                typeof(TImplementation),
                lifetime));

        services.TryAddEnumerable(
            new ServiceDescriptor(
                typeof(TService),
                typeof(TImplementation),
                lifetime));

        return services;
    }
}
