namespace Ais.GameEngine.Hooks.Abstractions;

public interface ILateUpdate : IHook
{
    void LateUpdate(float deltaTime);
}
