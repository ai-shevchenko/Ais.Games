namespace Ais.GameEngine.Hooks.Abstractions;

public interface IHook
{
    bool IsEnabled { get; }

    void Disable();
    void Enable();
}
