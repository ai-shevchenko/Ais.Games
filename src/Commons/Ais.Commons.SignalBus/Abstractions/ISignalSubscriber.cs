namespace Ais.Commons.SignalBus.Abstractions;

public interface ISignalSubscriber
{
    IDisposable Subscribe<TSignal>(Action<TSignal> handler)
        where TSignal : ISignal;

    IDisposable Subscribe<TSignal>(Func<TSignal, Task> handler)
        where TSignal : ISignal;

    IDisposable Subscribe<TSignal>(Func<TSignal, CancellationToken, Task> handler)
        where TSignal : ISignal;
}
