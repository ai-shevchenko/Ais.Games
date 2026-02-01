using System.Collections.Concurrent;

using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core;

internal sealed class HooksSource : IHooksSource
{
    private bool _disposed;

    private readonly ConcurrentDictionary<Type, (IHook Hook, int Order)> _hooks = [];
    private readonly IHookFactory _hookFactory;

    public HooksSource(IHookFactory hookFactory)
    {
        _hookFactory = hookFactory;
    }

    public IReadOnlyList<T> GetHooks<T>(bool enabledOnly = false)
        where T : class, IHook
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

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
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_hooks.TryGetValue(typeof(T), out var item))
        {
            return (T)item.Hook;
        }
        throw new InvalidOperationException($"The hook {typeof(T).Name} not found");
    }

    public void AddHook<T>()
        where T : class, IHook
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var hook = _hookFactory.CreateHook<T>();
        _hooks.TryAdd(typeof(T), (hook, int.MaxValue));
    }

    public void AddHook<T>(int order)
        where T : class, IHook
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var hook = _hookFactory.CreateHook<T>();
        _hooks.TryAdd(typeof(T), (hook, order));
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        foreach (var item in _hooks.Values)
        {
            if (item.Hook is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _hooks.Clear();
    }
}
