using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Tests.Fixtures;
using NSubstitute;

namespace Ais.GameEngine.Core.Tests.GameEngine;

public sealed class GameEnginePauseResumeTests : IDisposable
{
    private readonly GameLoopFactoryFixture _fixture = new();

    [Fact(DisplayName = "Проверка паузирование всех игровых циклов")]
    public async Task PauseAllAsync_RunningGameLoops_PausesAll()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("Loop1");
        _fixture.SetupFactoryToCreateLoop("Loop2");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var loop1 = engine.CreateGameLoop("Loop1");
        var loop2 = engine.CreateGameLoop("Loop2");

        loop1.IsRunning.Returns(true);
        loop2.IsRunning.Returns(true);

        // Act
        await engine.PauseAllAsync();

        // Assert
        await loop1.Received(1).PauseAsync(Arg.Any<CancellationToken>());
        await loop2.Received(1).PauseAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка возобновление всех паузированных циклов")]
    public async Task ResumeAllAsync_PausedGameLoops_ResumesAll()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("Loop1");
        _fixture.SetupFactoryToCreateLoop("Loop2");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var loop1 = engine.CreateGameLoop("Loop1");
        var loop2 = engine.CreateGameLoop("Loop2");

        loop1.IsPaused.Returns(true);
        loop2.IsPaused.Returns(true);

        // Act
        await engine.ResumeAllAsync();

        // Assert
        await loop1.Received(1).ResumeAsync(Arg.Any<CancellationToken>());
        await loop2.Received(1).ResumeAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка паузирование пропускает не-работающие циклы")]
    public async Task PauseAllAsync_WithStoppedLoop_OnlyPausesRunning()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("RunningLoop");
        _fixture.SetupFactoryToCreateLoop("StoppedLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var runningLoop = engine.CreateGameLoop("RunningLoop");
        var stoppedLoop = engine.CreateGameLoop("StoppedLoop");

        runningLoop.IsRunning.Returns(true);
        stoppedLoop.IsRunning.Returns(false);

        // Act
        await engine.PauseAllAsync();

        // Assert
        await runningLoop.Received(1).PauseAsync(Arg.Any<CancellationToken>());
        await stoppedLoop.DidNotReceive().PauseAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка возобновление пропускает не-паузированные циклы")]
    public async Task ResumeAllAsync_WithRunningLoop_OnlyResumePaused()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("PausedLoop");
        _fixture.SetupFactoryToCreateLoop("RunningLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var pausedLoop = engine.CreateGameLoop("PausedLoop");
        var runningLoop = engine.CreateGameLoop("RunningLoop");

        pausedLoop.IsPaused.Returns(true);
        runningLoop.IsPaused.Returns(false);

        // Act
        await engine.ResumeAllAsync();

        // Assert
        await pausedLoop.Received(1).ResumeAsync(Arg.Any<CancellationToken>());
        await runningLoop.DidNotReceive().ResumeAsync(Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
