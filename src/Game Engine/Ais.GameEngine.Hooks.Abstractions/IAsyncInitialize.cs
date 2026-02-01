namespace Ais.GameEngine.Hooks.Abstractions;

public interface IAsyncInitialize : IAsyncHook
{
    Task InitializeAsync(CancellationToken cancellationToken);
}