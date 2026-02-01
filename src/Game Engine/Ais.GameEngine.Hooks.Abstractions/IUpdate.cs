namespace Ais.GameEngine.Hooks.Abstractions;

public interface IUpdate : IHook
{
    void Update(float deltaTime);
}
