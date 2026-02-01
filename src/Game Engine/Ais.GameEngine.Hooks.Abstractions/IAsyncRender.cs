namespace Ais.GameEngine.Hooks.Abstractions;

public interface IAsyncRender : IAsyncHook
{
    Task RenderAsync(float interpolationFactor, CancellationToken cancellationToken);
}