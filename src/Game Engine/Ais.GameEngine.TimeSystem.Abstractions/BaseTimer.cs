using System.Diagnostics;

namespace Ais.GameEngine.TimeSystem.Abstractions;

public abstract class BaseTimer : IGameTimeSource, IGameTimerController, IDisposable
{
    private bool _disposed;
    private float _accumulator;
    private long _previousTicks;

    protected readonly GameTimerSettings Settings;
    protected readonly Stopwatch Timer = new();
    
    protected BaseTimer(GameTimerSettings settings)
    {
        Settings = settings;
    }

    /// <inheritdoc />
    public float InterpolationFactor => _accumulator / Settings.FixedDeltaTime;

    /// <inheritdoc />
    public float FixedDeltaTime => Settings.FixedDeltaTime * Scale;

    /// <inheritdoc />
    public bool ShouldFixedUpdate
    {
        get
        {
            if (_accumulator < Settings.FixedDeltaTime)
            {
                return false;
            }
            
            _accumulator -= Settings.FixedDeltaTime;
            return true;
        }
    }

    /// <inheritdoc />
    public float DeltaTime
    {
        get
        {
            var currentTime = Timer.ElapsedTicks;
            var elapsedTicks = currentTime - _previousTicks;

            _previousTicks = currentTime;

            var delta = (float)elapsedTicks / Stopwatch.Frequency;
            var scaledDelta = delta * Scale;

            if (delta > Settings.MaxFrameTime)
            {
                delta = Settings.MaxFrameTime;
            }   

            _accumulator += delta;

            return scaledDelta;
        }
    }

    /// <inheritdoc />
    public float TotalTime => (float)Timer.Elapsed.TotalSeconds;
    
    /// <inheritdoc />
    public float Scale { get; protected set; } = 1f;

    /// <inheritdoc />
    public bool IsRunning { get; protected set; }

    /// <inheritdoc />
    public void Start()
    {
        if (IsRunning)
        {
            return;
        }
        
        Timer.Start();
        IsRunning = true;
    }

    /// <inheritdoc />
    public virtual void Stop()
    {
        CheckDisposed();
        
        if (!IsRunning)
        {
            return;
        }

        Timer.Stop();
        IsRunning = false;
    }

    /// <inheritdoc />
    public virtual void Restart()
    {
        CheckDisposed();
        _accumulator = 0;
        _previousTicks = 0;
        Timer.Restart();
        IsRunning = true;
    }

    /// <inheritdoc />
    public virtual void SetScale(float scale)
    {
        CheckDisposed();
        ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0f);
        Scale = scale;
    }

    /// <inheritdoc />
    public virtual void ResetScale()
    {
        CheckDisposed();
        Scale = 1f;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Timer.Stop();
    }

    /// <inheritdoc />
    protected void CheckDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
