namespace Ais.GameEngine.Hooks.Abstractions;

public interface IAsyncDestroy : IAsyncHook
{
    Task OnDestroyAsync(CancellationToken cancellationToken = default);
}