using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Hooks;

internal sealed class HooksProvider : IHooksProvider
{
    private readonly IServiceProvider _gameServices;

    public HooksProvider(IServiceProvider gameServices)
    {
        _gameServices = gameServices;
    }

    public IReadOnlyList<T> GetHooks<T>(bool enabledOnly = false)
        where T : class, IHook
    {
        return GetHooks(enabledOnly)
            .OfType<T>()
            .ToArray();
    }

    public T GetHook<T>()
        where T : class, IHook
    {
        return _gameServices.GetService<T>()
               ?? throw new InvalidOperationException($"The hook {typeof(T).Name} not found");
    }

    private IEnumerable<IHook> GetHooks(bool enabledOnly = false)
    {
        var query = _gameServices.GetServices<IHook>();

        if (enabledOnly)
        {
            query = query.Where(h => h.IsEnabled);
        }

        return query;
    }
}
