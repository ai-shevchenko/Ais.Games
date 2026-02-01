using Ais.GameEngine.TimeSystem.Abstractions;
using Ais.GameEngine.Core.Settings;

using Microsoft.Extensions.Options;

namespace Ais.GameEngine.Core.TimeSystem;

public class GameTimer : BaseTimer, IGameTimer
{
    private readonly List<IFrameTimer> _frameTimers = [];

    public GameTimer(ITimerController parent, IOptionsMonitor<GameEngineSettings> settings)
        : base(settings.CurrentValue.TimerSettings)
    {
        Parent = parent;
    }

    public ITimerController Parent { get; }
    
    public IFrameTimer CreateFrameTimer()
    {
        var frameTimer = new FrameTimer(this, Settings);
        _frameTimers.Add(frameTimer);
        return frameTimer;
    }

    public override void Dispose()
    {
        base.Dispose();

        foreach (var timer in _frameTimers)
        {
            timer.Dispose();
        }
        _frameTimers.Clear();
        _frameTimers.TrimExcess();

        
    }
}