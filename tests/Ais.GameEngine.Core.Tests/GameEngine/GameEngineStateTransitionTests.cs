using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Tests.Fixtures;

namespace Ais.GameEngine.Core.Tests.GameEngine;

public sealed class GameEngineStateTransitionTests : IDisposable
{
    private readonly GameLoopFactoryFixture _fixture = new();

    [Fact(DisplayName = "Проверка начального состояния двигателя при создании")]
    public void Constructor_NoArguments_ReturnsNotInitializedState()
    {
        // Arrange
        // Act
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Assert
        Assert.Equal(EngineState.Idle, engine.State);
    }

    [Fact(DisplayName = "Проверка переход в Running при старте двигателя без циклов")]
    public async Task StartAsync_NoGameLoops_TransitionsToRunningState()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        using var cts = new CancellationTokenSource();

        // Act
        await engine.StartAsync(cts.Token);

        // Assert
        Assert.Equal(EngineState.Running, engine.State);
    }

    [Fact(DisplayName = "Проверка переход в Running при старте двигателя с одним циклом")]
    public async Task StartAsync_WithOneGameLoop_TransitionsToRunningState()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("MainLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var gameLoop = engine.CreateGameLoop("MainLoop");

        await gameLoop.StartAsync();

        using var cts = new CancellationTokenSource();

        // Act
        await engine.StartAsync(cts.Token);

        // Assert
        Assert.Equal(EngineState.Running, engine.State);

        // Cleanup
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
    }

    [Fact(DisplayName = "Проверка переход в Stopping при остановке работающего двигателя")]
    public async Task StopAsync_RunningEngine_TransitionsToStopped()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        using var cts = new CancellationTokenSource();
        await engine.StartAsync(cts.Token);

        // Act
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);

        // Assert
        Assert.Equal(EngineState.Stopped, engine.State);
    }

    [Fact(DisplayName = "Проверка ошибка при попытке старта двигателя не из Idle состояния")]
    public async Task StartAsync_EngineNotInIdleState_ThrowsInvalidOperationException()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        using var cts = new CancellationTokenSource();
        await engine.StartAsync(cts.Token);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => engine.StartAsync(cts.Token));
        Assert.Contains("Cannot start game engine", exception.Message);

        // Cleanup
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
    }

    [Fact(DisplayName = "Проверка идемпотентность остановки уже остановленного двигателя")]
    public async Task StopAsync_AlreadyStopped_DoesNotThrow()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        using var cts = new CancellationTokenSource();
        await engine.StartAsync(cts.Token);
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);

        // Act & Assert
        await engine.StopAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
        Assert.Equal(EngineState.Stopped, engine.State);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
