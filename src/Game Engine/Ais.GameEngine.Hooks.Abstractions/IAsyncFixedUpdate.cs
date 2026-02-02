namespace Ais.GameEngine.Hooks.Abstractions;

public interface IAsyncFixedUpdate : IAsyncHook
{
    Task FixedUpdateAsync(float fixedDeltaTime, CancellationToken cancellationToken);
}
