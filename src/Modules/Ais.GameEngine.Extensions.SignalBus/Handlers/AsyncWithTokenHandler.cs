namespace Ais.GameEngine.Extensions.SignalBus.Handlers;

internal sealed class AsyncWithTokenHandler : HandlerBase
{
    public override void Handle<TSignal>(TSignal signal, Delegate signalHandler)
    {
        HandleAsync(signal, signalHandler)
            .GetAwaiter()
            .GetResult();
    }

    public override Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler,
        CancellationToken cancellationToken = default)
    {
        return ((Func<TSignal, CancellationToken, Task>)signalHandler)(signal, cancellationToken);
    }
}
