namespace Ais.GameEngine.Core;

public class StateTransitionException : Exception
{
    public StateTransitionException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
