using System.Collections.Concurrent;

using Ais.GameEngine.TimeSystem.Abstractions;
using Ais.GameEngine.Core.Settings;

using Microsoft.Extensions.Options;

namespace Ais.GameEngine.Core.TimeSystem;

public class MainTimerController : BaseTimer, ITimerController, IDisposable
{
    private readonly List<IGameTimer> _timers = [];
    private readonly ConcurrentDictionary<string, IGameTimer>  _namedTimers = [];

    private readonly IOptionsMonitor<GameEngineSettings> _optionsMonitor;

    public MainTimerController(IOptionsMonitor<GameEngineSettings> settings)
        : base(settings.CurrentValue.TimerSettings)
    {
        _optionsMonitor = settings;
    }

    public override void SetScale(float scale)
    {
        base.SetScale(scale);

        foreach (var timer in _timers)
        {
            timer.SetScale(scale);
        }
        foreach (var timer in _namedTimers.Values)
        {
            timer.SetScale(scale);
        }
    }

    public override void ResetScale()
    {
        base.ResetScale();

        foreach (var timer in _timers)
        {
            timer.ResetScale();
        }
        foreach (var timer in _namedTimers.Values)
        {
            timer.ResetScale();
        }
    }

    public IGameTimer CreateChildTimer()
    {
        CheckDisposed();
        var timer = new GameTimer(this, _optionsMonitor);
        _timers.Add(timer);
        return timer;
    }

    public IGameTimer CreateChildTimer(string name)
    {
        CheckDisposed();
        var timer =  new GameTimer(this, _optionsMonitor);
        return _namedTimers.TryAdd(name, timer) 
            ? timer
            : throw new ArgumentException($"Cannot add timer with name {name}");
    }

    public IGameTimer GetChildTimer(string name)
    {
        CheckDisposed();
        return _namedTimers.TryGetValue(name, out var timer) 
            ? timer 
            : throw new ArgumentException($"Cannot find timer with name {name}");
    }

    public override void Dispose()
    {
        base.Dispose();

        foreach (var timer in _timers)
        {
            timer.Dispose();
        }
        foreach (var timer in _namedTimers.Values)
        {
            timer.Dispose();
        }

        
    }
}