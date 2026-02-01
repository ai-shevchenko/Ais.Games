using Ais.Commons.SignalBus.Abstractions;

namespace Ais.Commons.SignalBus.Handlers;

internal abstract class HandlerBase
{
    public abstract void Handle<TSignal>(TSignal signal, Delegate signalHandler)
        where TSignal : ISignal;

    public abstract Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default)
        where TSignal : ISignal;
}