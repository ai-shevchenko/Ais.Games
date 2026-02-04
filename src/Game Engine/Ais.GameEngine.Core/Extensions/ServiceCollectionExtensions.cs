using Ais.GameEngine.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Core.Extensions;

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
            typeof(TState),
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
            typeof(TInterceptor),
            lifetime));

        return services;
    }
}
