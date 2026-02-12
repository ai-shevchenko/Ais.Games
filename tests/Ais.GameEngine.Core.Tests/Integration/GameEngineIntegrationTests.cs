using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Tests.Fixtures;

using NSubstitute;

namespace Ais.GameEngine.Core.Tests.Integration;

public sealed class GameEngineIntegrationTests : IDisposable
{
    private readonly GameLoopFactoryFixture _fixture = new();

    [Fact(DisplayName = "Проверка полный жизненный цикл движка с одним циклом")]
    public async Task GameEngine_FullLifecycle_WithOneLoop_Succeeds()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("MainLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var gameLoop = engine.CreateGameLoop("MainLoop");

        using var cts = new CancellationTokenSource();

        // Act
        Assert.Equal(EngineState.Idle, engine.State);

        await engine.StartAsync(cts.Token);
        Assert.Equal(EngineState.Running, engine.State);

        await Task.Delay(100, cts.Token);

        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
        Assert.Equal(EngineState.Stopped, engine.State);

        // Assert
        Assert.Single(engine.GameLoops);
        Assert.Equal(gameLoop, engine.GameLoops[0]);
    }

    [Fact(DisplayName = "Проверка полный жизненный цикл с несколькими циклами")]
    public async Task GameEngine_FullLifecycle_WithMultipleLoops_Succeeds()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("Loop1");
        _fixture.SetupFactoryToCreateLoop("Loop2");
        _fixture.SetupFactoryToCreateLoop("Loop3");

        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.CreateGameLoop("Loop1");
        engine.CreateGameLoop("Loop2");
        engine.CreateGameLoop("Loop3");

        using var cts = new CancellationTokenSource();

        // Act
        await engine.StartAsync(cts.Token);

        // Assert
        Assert.Equal(3, engine.GameLoops.Count);
        Assert.Equal(EngineState.Running, engine.State);

        // Cleanup
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
    }

    [Fact(DisplayName = "Проверка создание и удаление цикла во время работы")]
    public async Task GameEngine_CreateAndRemoveLoop_DuringExecution_Succeeds()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("Loop1");
        _fixture.SetupFactoryToCreateLoop("Loop2");

        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.CreateGameLoop("Loop1");

        using var cts = new CancellationTokenSource();

        await engine.StartAsync(cts.Token);

        // Act
        engine.CreateGameLoop("Loop2");
        Assert.Equal(2, engine.GameLoops.Count);

        var removed = await engine.RemoveGameLoopAsync("Loop2", TimeSpan.FromSeconds(5), cts.Token);
        Assert.True(removed);
        Assert.Single(engine.GameLoops);

        // Cleanup
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
        engine.Dispose();
    }

    [Fact(DisplayName = "Проверка получение несуществующего цикла вызывает KeyNotFoundException")]
    public void GameEngine_GetNonExistentLoop_ThrowsKeyNotFoundException()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => engine.GetGameLoop("NonExistent"));
    }

    [Fact(DisplayName = "Проверка паузирование и возобновление всех циклов")]
    public async Task GameEngine_PauseResumeAllLoops_Succeeds()
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

        // Resume
        loop1.IsPaused.Returns(true);
        loop2.IsPaused.Returns(true);

        await engine.ResumeAllAsync();

        await loop1.Received(1).ResumeAsync(Arg.Any<CancellationToken>());
        await loop2.Received(1).ResumeAsync(Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
