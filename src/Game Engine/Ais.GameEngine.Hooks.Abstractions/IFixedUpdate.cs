namespace Ais.GameEngine.Hooks.Abstractions;

public interface IFixedUpdate : IHook
{
    void FixedUpdate(float fixedDeltaTime);
}
