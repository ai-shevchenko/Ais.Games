namespace Ais.GameEngine.Hooks.Abstractions;

public abstract class BaseHook : IHook
{
    public bool IsEnabled { get; private set; } = true;

    public virtual void Disable()
    {
        IsEnabled = false;
    }

    public void Enable()
    {
        IsEnabled = true;
    }
}
