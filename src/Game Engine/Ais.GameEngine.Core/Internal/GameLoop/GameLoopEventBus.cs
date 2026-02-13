using System.Collections.Concurrent;

using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameLoopEventBus : IGameLoopEventBus, IDisposable
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ConcurrentDictionary<Type, List<SubscriptionInfo>> _subscribers = new();
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _lock.Dispose();
    }

    public IDisposable Subscribe<TEvent>(string? loopName, Func<TEvent, CancellationToken, Task> handler)
        where TEvent : IGameLoopEvent
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _lock.EnterWriteLock();
        try
        {
            var key = typeof(TEvent);
            var list = _subscribers.GetOrAdd(key, _ => []);

            var subscriptionInfo = new SubscriptionInfo(loopName, handler);
            list.Add(subscriptionInfo);

            return new Unsubscriber(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    if (_subscribers.TryGetValue(key, out var handlers))
                    {
                        handlers.Remove(subscriptionInfo);
                        if (handlers.Count == 0)
                        {
                            _subscribers.TryRemove(key, out _);
                        }
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            });
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public async Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IGameLoopEvent
    {
        _lock.EnterReadLock();
        try
        {
            if (_subscribers.TryGetValue(typeof(TEvent), out var handlers))
            {
                var tasks = new List<Task>(handlers.Count);

                foreach (var subscription in handlers)
                {
                    if (string.IsNullOrWhiteSpace(evt.TargetLoopName) || evt.TargetLoopName == subscription.LoopName)
                    {
                        tasks.Add(subscription.Invoke(evt, cancellationToken));
                    }
                }

                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Publish<TEvent>(TEvent evt)
        where TEvent : IGameLoopEvent
    {
        PublishAsync(evt, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    private sealed class SubscriptionInfo
    {
        private readonly Delegate _handler;

        public SubscriptionInfo(string? loopName, Delegate handler)
        {
            LoopName = loopName;
            _handler = handler;
        }

        public string? LoopName { get; }

        public Task Invoke<TEvent>(TEvent evt, CancellationToken cancellationToken)
            where TEvent : IGameLoopEvent
        {
            if (_handler is Func<TEvent, CancellationToken, Task> typedHandler)
            {
                return typedHandler(evt, cancellationToken);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly Action _unsubscribe;

        public Unsubscriber(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe();
        }
    }
}
