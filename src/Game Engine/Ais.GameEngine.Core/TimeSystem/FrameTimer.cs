using Ais.GameEngine.TimeSystem.Abstractions;

namespace Ais.GameEngine.Core.TimeSystem;

public class FrameTimer : BaseFrameTimer
{
    public FrameTimer(IGameTimer timer, GameTimerSettings settings) 
        : base(timer, settings)
    {
    }
}