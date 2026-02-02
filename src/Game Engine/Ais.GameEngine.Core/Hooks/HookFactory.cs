using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Hooks;

internal sealed class HookFactory : IHookFactory
{
    private readonly IServiceProvider _gameServices;

    public HookFactory(IServiceProvider gameServices)
    {
        _gameServices = gameServices;
    }

    public T CreateHook<T>()
        where T : class, IHook
    {
        return ActivatorUtilities.CreateInstance<T>(_gameServices);
    }

    public IHook CreateHook(Type hookType)
    {
        if (!typeof(IGameLoopState).IsAssignableFrom(hookType))
        {
            throw new ArgumentException($"Type must implement {nameof(IHook)}", nameof(hookType));
        }

        return (IHook)ActivatorUtilities.CreateInstance(_gameServices, hookType);
    }
}
