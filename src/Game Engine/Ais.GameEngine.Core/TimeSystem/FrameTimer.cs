using System.Diagnostics;

using Ais.GameEngine.TimeSystem.Abstractions;

namespace Ais.GameEngine.Core.TimeSystem;

internal sealed class FrameTimer : IFrameTimer, IDisposable
{
    private bool _disposed;

    private readonly Stopwatch _timer = new();

    public FrameTimer(IGameTimer timer, GameTimerSettings settings)
    {
        GameTimer = timer;
        TargetFrameRate = settings.TargetFrameRate;
    }

    /// <inheritdoc />
    public IGameTimer GameTimer { get; }

    /// <inheritdoc />
    public float TargetFrameRate { get; }

    /// <inheritdoc />
    public float Elapsed => (float)_timer.Elapsed.TotalSeconds;

    /// <inheritdoc />
    public float FrameRate => 1.0f / FrameTime;

    /// <inheritdoc />
    public float FrameTime => (float)_timer.ElapsedTicks / Stopwatch.Frequency;

    /// <inheritdoc />
    public float GetSleepTime()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (TargetFrameRate == 0)
        {
            return 0;
        }

        var targetFrameTime = 1f / TargetFrameRate;
        var frameTime = FrameTime;

        if (frameTime >= targetFrameTime)
        {
            return 0;
        }

        var sleepTime = targetFrameTime - frameTime;

        if (sleepTime > 0)
        {
            return sleepTime;
        }

        return 0;
    }

    /// <inheritdoc />
    public void Restart()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _timer.Restart();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _timer.Stop();
    }
}