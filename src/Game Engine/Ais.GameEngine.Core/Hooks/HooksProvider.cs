using System.Collections.Concurrent;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core.Hooks;

internal sealed class HooksSource : IHooksSource
{
    public IReadOnlyList<T> GetHooks<T>(bool enabledOnly = false)
        where T : class, IHook
    {
        var quey = _hooks.Values
            .OrderBy(x => x.Order)
            .Select(x => x.Hook)
            .OfType<T>();

        if (enabledOnly)
        {
            quey = quey.Where(h => h.IsEnabled);
        }

        return [.. quey];
    }

    public T GetHook<T>()
        where T : class, IHook
    {
        if (_hooks.TryGetValue(typeof(T), out var item))
        {
            return (T)item.Hook;
        }

        throw new InvalidOperationException($"The hook {typeof(T).Name} not found");
    }
}
