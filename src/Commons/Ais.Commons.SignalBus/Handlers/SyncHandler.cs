namespace Ais.Commons.SignalBus.Handlers;

internal sealed class SyncHandler : HandlerBase
{
    public override void Handle<TSignal>(TSignal signal, Delegate signalHandler)
    {
        ((Action<TSignal>)signalHandler)(signal);
    }

    public override Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Handle(signal, signalHandler), cancellationToken);
    }
}
