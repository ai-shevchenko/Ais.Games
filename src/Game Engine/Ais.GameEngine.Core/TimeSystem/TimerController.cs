using System.Collections.Concurrent;

using Ais.GameEngine.Core.Settings;
using Ais.GameEngine.TimeSystem.Abstractions;

using Microsoft.Extensions.Options;

namespace Ais.GameEngine.Core.TimeSystem;

public sealed class TimerController : ITimerController, IDisposable
{
    private readonly ConcurrentDictionary<string, IGameTimer> _namedTimers = [];
    private readonly IOptionsMonitor<GameEngineSettings> _optionsMonitor;

    private readonly List<IGameTimer> _timers = [];
    private bool _disposed;

    public TimerController(IOptionsMonitor<GameEngineSettings> settings)
    {
        _optionsMonitor = settings;
    }

    /// <inheritdoc />
    public float Scale { get; private set; }

    /// <inheritdoc />
    public bool IsRunning { get; private set; }

    /// <inheritdoc />
    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var timer in _timers)
        {
            timer.Start();
        }

        foreach (var timer in _namedTimers.Values)
        {
            timer.Start();
        }

        IsRunning = true;
    }

    /// <inheritdoc />
    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var timer in _timers)
        {
            timer.Stop();
        }

        foreach (var timer in _namedTimers.Values)
        {
            timer.Stop();
        }

        IsRunning = false;
    }

    /// <inheritdoc />
    public void Restart()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var timer in _timers)
        {
            timer.Restart();
        }

        foreach (var timer in _namedTimers.Values)
        {
            timer.Restart();
        }

        IsRunning = true;
    }

    /// <inheritdoc />
    public void SetScale(float scale)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var timer in _timers)
        {
            timer.SetScale(scale);
        }

        foreach (var timer in _namedTimers.Values)
        {
            timer.SetScale(scale);
        }
    }

    /// <inheritdoc />
    public void ResetScale()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var timer in _timers)
        {
            timer.ResetScale();
        }

        foreach (var timer in _namedTimers.Values)
        {
            timer.ResetScale();
        }
    }

    /// <inheritdoc />
    public IGameTimer CreateChildTimer()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var timer = new GameTimer(this, _optionsMonitor.CurrentValue.TimerSettings);
        _timers.Add(timer);
        return timer;
    }

    /// <inheritdoc />
    public IGameTimer CreateChildTimer(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var timer = new GameTimer(this, _optionsMonitor.CurrentValue.TimerSettings);
        return _namedTimers.TryAdd(name, timer)
            ? timer
            : throw new ArgumentException($"Cannot add timer with name {name}");
    }

    /// <inheritdoc />
    public IGameTimer GetChildTimer(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _namedTimers.TryGetValue(name, out var timer)
            ? timer
            : throw new ArgumentException($"Cannot find timer with name {name}");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

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
