using Ais.GameEngine.Core.Internal.GameLoop;
using Ais.GameEngine.Core.Tests.Fixtures;

namespace Ais.GameEngine.Core.Tests.EventBus;

public sealed class GameLoopEventBusPublishingTests : IDisposable
{
    private readonly GameLoopEventBusFixture _fixture = new();

    [Fact(DisplayName = "Проверка публикация события асинхронно")]
    public async Task PublishAsync_WithSubscriber_CallsHandler()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var handlerCalled = false;
        var receivedEvent = default(GameLoopEventBusFixture.TestGameLoopEvent);

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            handlerCalled = true;
            receivedEvent = evt;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("TestLoop", Handler);
        var evt = _fixture.CreateTestEvent();

        // Act
        await eventBus.PublishAsync(evt);

        // Assert
        Assert.True(handlerCalled);
        Assert.Same(evt, receivedEvent);
    }

    [Fact(DisplayName = "Проверка публикация события синхронно")]
    public void Publish_WithSubscriber_CallsHandler()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var handlerCalled = false;

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            handlerCalled = true;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("TestLoop", Handler);
        var evt = _fixture.CreateTestEvent();

        // Act
        eventBus.Publish(evt);

        // Assert
        Assert.True(handlerCalled);
    }

    [Fact(DisplayName = "Проверка публикация события без подписчиков не вызывает исключение")]
    public async Task PublishAsync_NoSubscribers_DoesNotThrow()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var evt = _fixture.CreateTestEvent();

        // Act & Assert
        await eventBus.PublishAsync(evt);
    }

    [Fact(DisplayName = "Проверка публикация с targetLoopName фильтрует подписчиков")]
    public async Task PublishAsync_WithTargetLoopName_OnlyTargetReceivesEvent()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var loop1Called = false;
        var loop2Called = false;

        async Task Handler1(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            loop1Called = true;
            await Task.CompletedTask;
        }

        async Task Handler2(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            loop2Called = true;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("Loop1", Handler1);
        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("Loop2", Handler2);

        var evt = _fixture.CreateTestEvent(sourceLoopName: "SourceLoop", targetLoopName: "Loop1");

        // Act
        await eventBus.PublishAsync(evt);

        // Assert
        Assert.True(loop1Called);
        Assert.False(loop2Called);
    }

    [Fact(DisplayName = "Проверка публикация без targetLoopName достигает всех подписчиков")]
    public async Task PublishAsync_NoTargetLoopName_AllSubscribersReceiveEvent()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var loop1Called = false;
        var loop2Called = false;

        async Task Handler1(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            loop1Called = true;
            await Task.CompletedTask;
        }

        async Task Handler2(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            loop2Called = true;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("Loop1", Handler1);
        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("Loop2", Handler2);

        var evt = _fixture.CreateTestEvent(sourceLoopName: "SourceLoop", targetLoopName: null);

        // Act
        await eventBus.PublishAsync(evt);

        // Assert
        Assert.True(loop1Called);
        Assert.True(loop2Called);
    }

    [Fact(DisplayName = "Проверка публикация передает CancellationToken в обработчик")]
    public async Task PublishAsync_WithCancellationToken_PassesTokenToHandler()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var receivedToken = CancellationToken.None;

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            receivedToken = ct;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("TestLoop", Handler);
        var evt = _fixture.CreateTestEvent();

        using var cts = new CancellationTokenSource();

        // Act
        await eventBus.PublishAsync(evt, cts.Token);

        // Assert
        Assert.Equal(cts.Token, receivedToken);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
