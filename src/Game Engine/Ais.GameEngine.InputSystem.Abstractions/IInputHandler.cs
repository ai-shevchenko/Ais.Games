namespace Ais.GameEngine.InputSystem.Abstractions;

public interface IInputHandler
{
    void Enable();

    void Disable();

    void Handle();
}