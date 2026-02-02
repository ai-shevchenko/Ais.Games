using System.Diagnostics;
using Ais.GameEngine.TimeSystem.Abstractions;

namespace Ais.GameEngine.Core.TimeSystem;

internal sealed class GameTimer : IGameTimer
{
    private readonly List<IFrameTimer> _frameTimers = [];
    private readonly GameTimerSettings _settings;
    private readonly Stopwatch _timer = new();
    private float _accumulator;
    private bool _disposed;
    private long _previousTicks;

    public GameTimer(ITimerController parent, GameTimerSettings settings)
    {
        Parent = parent;
        _settings = settings;
    }

    /// <inheritdoc />
    public float InterpolationFactor => _accumulator / _settings.FixedDeltaTime;

    /// <inheritdoc />
    public float FixedDeltaTime => _settings.FixedDeltaTime * Scale;

    /// <inheritdoc />
    public bool ShouldFixedUpdate
    {
        get
        {
            if (_accumulator < _settings.FixedDeltaTime)
            {
                return false;
            }

            _accumulator -= _settings.FixedDeltaTime;
            return true;
        }
    }

    /// <inheritdoc />
    public float DeltaTime
    {
        get
        {
            var currentTime = _timer.ElapsedTicks;
            var elapsedTicks = currentTime - _previousTicks;

            _previousTicks = currentTime;

            var delta = (float)elapsedTicks / Stopwatch.Frequency;
            var scaledDelta = delta * Scale;

            if (delta > _settings.MaxFrameTime)
            {
                delta = _settings.MaxFrameTime;
            }

            _accumulator += delta;

            return scaledDelta;
        }
    }

    /// <inheritdoc />
    public float TotalTime => (float)_timer.Elapsed.TotalSeconds;

    /// <inheritdoc />
    public float Scale { get; private set; } = 1f;

    /// <inheritdoc />
    public bool IsRunning { get; private set; }


    /// <inheritdoc />
    public ITimerController Parent { get; }

    /// <inheritdoc />
    public IFrameTimer CreateFrameTimer()
    {
        var frameTimer = new FrameTimer(this, _settings);
        _frameTimers.Add(frameTimer);
        return frameTimer;
    }

    /// <inheritdoc />
    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        _timer.Start();
        IsRunning = true;
    }

    /// <inheritdoc />
    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!IsRunning)
        {
            return;
        }

        _timer.Stop();
        IsRunning = false;
    }

    /// <inheritdoc />
    public void Restart()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _accumulator = 0;
        _previousTicks = 0;
        _timer.Restart();
        IsRunning = true;
    }

    /// <inheritdoc />
    public void SetScale(float scale)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0f);

        Scale = scale;
    }

    /// <inheritdoc />
    public void ResetScale()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Scale = 1f;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (var timer in _frameTimers)
        {
            timer.Dispose();
        }

        _frameTimers.Clear();
        _frameTimers.TrimExcess();

        _timer.Stop();
    }
}
