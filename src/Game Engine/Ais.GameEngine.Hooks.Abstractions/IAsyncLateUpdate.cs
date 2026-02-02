namespace Ais.GameEngine.Hooks.Abstractions;

public interface IAsyncLateUpdate : IAsyncHook
{
    Task LateUpdateAsync(float deltaTime, CancellationToken cancellationToken);
}
