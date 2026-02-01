using System.Diagnostics;

namespace Ais.GameEngine.TimeSystem.Abstractions;

public abstract class BaseFrameTimer : IFrameTimer, IDisposable
{
    protected readonly Stopwatch Timer = new();

    private bool _disposed;

    public BaseFrameTimer(IGameTimer timer, GameTimerSettings settings)
    {
        GameTimer = timer;
        TargetFrameRate = settings.TargetFrameRate;
    }

    public IGameTimer GameTimer { get; }

    public float TargetFrameRate { get; }
    public float Elapsed => (float)Timer.Elapsed.TotalSeconds;

    public float FrameRate => 1.0f / FrameTime;

    public float FrameTime => (float)Timer.ElapsedTicks / Stopwatch.Frequency;

    public float GetSleepTime()
    {
        CheckDisposed();

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

    public void Restart()
    {
        CheckDisposed();
        Timer.Restart();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            Timer.Stop();
        }
    }

    private void CheckDisposed()
    {
        if (!_disposed)
        {
            return;
        }
        throw new ObjectDisposedException(GetType().Name);
    }
}
