namespace Ais.GameEngine.Extensions.SignalBus.Abstractions;

public interface ISignalBus : ISignalSubscriber, ISignalPublisher, IDisposable
{
    void Unsubscribe<TSignal>(Action<TSignal> signalHandler)
        where TSignal : ISignal;

    void Unsubscribe<TSignal>(Func<TSignal, Task> asyncSignalHandler)
        where TSignal : ISignal;

    void Unsubscribe<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler)
        where TSignal : ISignal;
}
