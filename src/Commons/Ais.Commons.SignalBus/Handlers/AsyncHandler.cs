namespace Ais.Commons.SignalBus.Handlers;

internal sealed class AsyncHandler : HandlerBase
{
    public override void Handle<TSignal>(TSignal signal, Delegate signalHandler)
    {
        HandleAsync(signal, signalHandler)
            .GetAwaiter()
            .GetResult();
    }

    public override Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return ((Func<TSignal, Task>)signalHandler)(signal);
    }
}
