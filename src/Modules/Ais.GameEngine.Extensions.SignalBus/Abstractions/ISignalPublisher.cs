namespace Ais.GameEngine.Extensions.SignalBus.Abstractions;

public interface ISignalPublisher
{
    void Publish<TSignal>(TSignal signal)
        where TSignal : ISignal;

    Task PublishAsync<TSignal>(TSignal signal, CancellationToken cancellationToken = default)
        where TSignal : ISignal;
}
