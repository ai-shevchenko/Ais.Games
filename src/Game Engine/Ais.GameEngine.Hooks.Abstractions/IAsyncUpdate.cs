namespace Ais.GameEngine.Hooks.Abstractions;

public interface IAsyncUpdate : IAsyncHook
{
    Task UpdateAsync(float deltaTime, CancellationToken cancellationToken);
}

