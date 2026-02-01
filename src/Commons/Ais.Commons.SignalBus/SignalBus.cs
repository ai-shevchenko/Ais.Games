using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Ais.Commons.SignalBus.Abstractions;

namespace Ais.Commons.SignalBus;

public sealed class SignalBus : ISignalBus
{
    private readonly ConcurrentDictionary<Type, List<HandlerEntry>> _handlers = new();
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ILogger<SignalBus> _logger;

    public SignalBus(ILogger<SignalBus> logger)
    {
        _logger = logger;
    }

    public IDisposable Subscribe<TSignal>(Func<TSignal, Task> handler) 
        where TSignal : ISignal
    { 
        RegisterInternal(typeof(TSignal), handler);
        return new Subscription<TSignal>(this, handler);
    }

    public IDisposable Subscribe<TSignal>(Func<TSignal, CancellationToken, Task> handler) 
        where TSignal : ISignal
    { 
        RegisterInternal(typeof(TSignal), handler);
        return new Subscription<TSignal>(this, handler);
    }

    public IDisposable Subscribe<TSignal>(Action<TSignal> handler) where TSignal : ISignal
    { 
        RegisterInternal(typeof(TSignal), handler);
        return new Subscription<TSignal>(this, handler);
    }

    public async Task PublishAsync<TSignal>(TSignal signal, CancellationToken cancellationToken = default) 
        where TSignal : ISignal
    {
        var handlersToInvoke = FindSignalHandlers<TSignal>();

        if (handlersToInvoke.Count == 0)
        {
            return;
        }

        var tasks = handlersToInvoke
            .Select(handler => HandlersManager<TSignal>.HandleAsync(signal, handler, cancellationToken));
        
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fault handling");
        }
    }

    public void Publish<TSignal>(TSignal signal) 
        where TSignal : ISignal
    {
        var handlersToInvoke = FindSignalHandlers<TSignal>();

        if (handlersToInvoke.Count == 0)
        {
            return;
        }

        foreach (var handler in handlersToInvoke)
        {
            try
            {
                HandlersManager<TSignal>.Handle(signal, handler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fault handling");
            }
        }
    }

    public void Unsubscribe<TSignal>(Action<TSignal> signalHandler) where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), signalHandler);
    }

    public void Unsubscribe<TSignal>(Func<TSignal, Task> asyncSignalHandler)
    where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    public void Unsubscribe<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler) where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    private void UnregisterInternal(Type signalType, Delegate signalHandler)
    {
        var hashCode = RuntimeHelpers.GetHashCode(signalHandler);

        _lock.EnterWriteLock();
        try
        {
            if (!_handlers.TryGetValue(signalType, out var entries))
            {
                return;
            }
            
            for (var i = entries.Count - 1; i >= 0; i--)
            {
                if (entries[i].HashCode == hashCode ||
                    (entries[i].Reference.TryGetTarget(out var target) &&
                     target == signalHandler))
                {
                    entries.RemoveAt(i);
                    break;
                }
            }

            if (entries.Count == 0)
            {
                _handlers.TryRemove(signalType, out _);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private void RegisterInternal(Type signalType, Delegate signalHandler)
    {
        var entry = new HandlerEntry(signalHandler);

        _lock.EnterWriteLock();
        try
        {
            var list = _handlers.GetOrAdd(signalType, _ => []);
            list.Add(entry);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private List<Delegate> FindSignalHandlers<TSignal>() 
        where TSignal : ISignal
    {
        List<Delegate> handlersToInvoke = [];

        _lock.EnterReadLock();
        try
        {
            if (_handlers.TryGetValue(typeof(TSignal), out var entries))
            {
                foreach (var entry in entries)
                {
                    if (entry.Reference.TryGetTarget(out var handler))
                    {
                        handlersToInvoke.Add(handler);
                    }
                }
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return handlersToInvoke;
    }

    public void Dispose()
    {
        _lock.EnterWriteLock();
        try
        {
            _handlers.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
            _lock.Dispose();
        }
    }

    private sealed class HandlerEntry
    {
        public WeakReference<Delegate> Reference { get; }
        public int HashCode { get; }

        public HandlerEntry(Delegate handler)
        {
            Reference = new WeakReference<Delegate>(handler);
            HashCode = RuntimeHelpers.GetHashCode(handler);
        }
    }
    
    private sealed class Subscription<T> : IDisposable
        where T : ISignal
    {
        private readonly ISignalBus _bus;
        private readonly Delegate _handler;
        
        public Subscription(ISignalBus bus, Delegate handler)
        {
            _bus = bus;
            _handler = handler;
        }

        public void Dispose()
        {
            switch (_handler)
            {
                case Action<T> handler:
                    _bus.Unsubscribe(handler);
                    return;
                case Func<T, Task> asyncHandler:
                    _bus.Unsubscribe(asyncHandler);
                    return;
                case Func<T, CancellationToken, Task> asyncTokeHandler:
                    _bus.Unsubscribe(asyncTokeHandler);
                    break;
            }
        }
    }
}