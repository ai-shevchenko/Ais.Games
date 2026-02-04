using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Modules.Abstractions.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Добавить состояние
    /// </summary>
    /// <param name="services">Список сервисов</param>
    /// <param name="lifetime">Жизненный цикл состояния</param>
    /// <typeparam name="TState">Тип состояния</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddState<TState>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TState : class, IGameLoopState
    {
        services.TryAdd(new ServiceDescriptor(
            typeof(TState),
            typeof(TState),
            lifetime));

        services.TryAdd(new ServiceDescriptor(
            typeof(IGameLoopState),
            sp => sp.GetRequiredService<TState>(),
            lifetime));

        return services;
    }

    /// <summary>
    ///     Добавить перехватчик состояний
    /// </summary>
    /// <param name="services">Список сервисов</param>
    /// <param name="lifetime">Жизненный цикл состояния</param>
    /// <typeparam name="TInterceptor">Тип перехватчика</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddStateInterceptor<TInterceptor>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterceptor : class, IGameLoopStateInterceptor
    {
        services.TryAdd(new ServiceDescriptor(
            typeof(TInterceptor),
            typeof(TInterceptor),
            lifetime));

        services.TryAdd(new ServiceDescriptor(
            typeof(IGameLoopStateInterceptor),
            sp => sp.GetRequiredService<TInterceptor>(),
            lifetime));

        return services;
    }

    /// <summary>
    ///     Добавить хук жизненного цикла
    /// </summary>
    /// <param name="services">Список сервисов</param>
    /// <param name="lifetime"></param>
    /// <typeparam name="THook"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddHook<THook>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THook : class, IHook
    {
        services.TryAdd(new ServiceDescriptor(
            typeof(THook),
            typeof(THook),
            lifetime));

        services.Add(new ServiceDescriptor(
            typeof(IHook),
            sp => sp.GetRequiredService<THook>(),
            lifetime));

        return services;
    }
}
