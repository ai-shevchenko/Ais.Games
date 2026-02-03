using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.GameEngine.Extensions.SignalBus.Handlers;

namespace Ais.GameEngine.Extensions.SignalBus;

internal static class HandlersManager<TSignal>
    where TSignal : ISignal
{
    private static readonly Dictionary<Type, Func<HandlerBase>> _handlers = new()
    {
        { typeof(Action<TSignal>), () => new SyncHandler() },
        { typeof(Func<TSignal, Task>), () => new AsyncHandler() },
        { typeof(Func<TSignal, CancellationToken, Task>), () => new AsyncWithTokenHandler() }
    };

    public static void Handle(TSignal signal, Delegate signalHandler)
    {
        GetTriggerHandler(signalHandler.GetType()).Handle(signal, signalHandler);
    }

    public static Task HandleAsync(TSignal signal, Delegate signalHandler,
        CancellationToken cancellationToken = default)
    {
        return GetTriggerHandler(signalHandler.GetType()).HandleAsync(signal, signalHandler, cancellationToken);
    }

    private static HandlerBase GetTriggerHandler(Type handlerType)
    {
        if (!_handlers.TryGetValue(handlerType, out var triggerHandlerFactory))
        {
            throw new ArgumentOutOfRangeException(nameof(handlerType));
        }

        return triggerHandlerFactory();
    }
}
