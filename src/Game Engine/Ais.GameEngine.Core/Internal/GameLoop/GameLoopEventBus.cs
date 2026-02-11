using System.Collections.Concurrent;

using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameLoopEventBus : IGameLoopEventBus
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<Delegate>> _subscribers = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public IDisposable Subscribe<TEvent>(string loopName, Func<TEvent, CancellationToken, Task> handler)
        where TEvent : IGameLoopEvent
    {
        _lock.EnterWriteLock();
        try
        {
            var key = typeof(TEvent);
            var bag = _subscribers.GetOrAdd(key, _ => []);

            var wrappedHandler = new Func<TEvent, CancellationToken, Task>(async (evt, ct) =>
            {
                if (evt.TargetLoopName == null || evt.TargetLoopName == loopName)
                {
                    await handler(evt, ct);
                }
            });

            bag.Add(wrappedHandler);

            return new Unsubscriber(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    var temp = new List<Delegate>();
                    var removed = false;

                    while (bag.TryTake(out var item))
                    {
                        if (!removed && item == (Delegate)wrappedHandler)
                        {
                            removed = true;
                            continue;
                        }

                        temp.Add(item);
                    }

                    foreach (var it in temp)
                    {
                        bag.Add(it);
                    }

                    if (bag.IsEmpty)
                    {
                        _subscribers.TryRemove(key, out _);
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
                var tasks = handlers
                    .Cast<Delegate>()
                    .OfType<Func<TEvent, CancellationToken, Task>>()
                    .Select(h => Task.Run(async () => await h(evt, cancellationToken).ConfigureAwait(false), cancellationToken));

                await Task.WhenAll(tasks);
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly Action _unsubscribe;

        public Unsubscriber(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose() => _unsubscribe();
    }
}
