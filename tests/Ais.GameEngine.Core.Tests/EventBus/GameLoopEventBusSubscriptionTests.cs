using Ais.GameEngine.Core.Internal.GameLoop;
using Ais.GameEngine.Core.Tests.Fixtures;

namespace Ais.GameEngine.Core.Tests.EventBus;

public sealed class GameLoopEventBusSubscriptionTests : IDisposable
{
    private readonly GameLoopEventBusFixture _fixture = new();

    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Fact(DisplayName = "Проверка подписка на события")]
    public async Task Subscribe_WithValidHandler_ReturnsDisposable()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var handlerCalled = false;

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            handlerCalled = true;
            await Task.CompletedTask;
        }

        // Act
        var subscription = eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>(
            "TestLoop",
            Handler);

        await eventBus.PublishAsync(new GameLoopEventBusFixture.TestGameLoopEvent());

        // Assert
        Assert.NotNull(subscription);
        Assert.True(handlerCalled);
    }

    [Fact(DisplayName = "Проверка множественные подписчики на одно событие")]
    public async Task Subscribe_MultipleSubscribers_AllReceiveEvent()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var subscriber1Called = false;
        var subscriber2Called = false;

        async Task Handler1(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            subscriber1Called = true;
            await Task.CompletedTask;
        }

        async Task Handler2(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            subscriber2Called = true;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("Loop1", Handler1);
        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("Loop2", Handler2);

        var evt = _fixture.CreateTestEvent("Loop1", null);

        // Act
        await eventBus.PublishAsync(evt);

        // Assert
        Assert.True(subscriber1Called);
        Assert.True(subscriber2Called);
    }

    [Fact(DisplayName = "Проверка отписка от события удаляет обработчик")]
    public async Task Unsubscribe_AfterDisposingSubscription_HandlerNotCalled()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();
        var handlerCalled = false;

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            handlerCalled = true;
            await Task.CompletedTask;
        }

        var subscription = eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>(
            "TestLoop",
            Handler);

        var evt = _fixture.CreateTestEvent();

        // Act
        subscription.Dispose();
        await eventBus.PublishAsync(evt);

        // Assert
        Assert.False(handlerCalled);
    }

    [Fact(DisplayName = "Проверка отписка идемпотентна")]
    public void Unsubscribe_CalledTwice_DoesNotThrow()
    {
        // Arrange
        using var eventBus = new GameLoopEventBus();

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            await Task.CompletedTask;
        }

        var subscription = eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>(
            "TestLoop",
            Handler);

        // Act & Assert
        subscription.Dispose();
        subscription.Dispose();
    }
}
