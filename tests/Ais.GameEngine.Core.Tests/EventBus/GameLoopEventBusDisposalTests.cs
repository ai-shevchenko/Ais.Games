using Ais.GameEngine.Core.Internal.GameLoop;
using Ais.GameEngine.Core.Tests.Fixtures;

namespace Ais.GameEngine.Core.Tests.EventBus;

public sealed class GameLoopEventBusDisposalTests : IDisposable
{
    private readonly GameLoopEventBusFixture _fixture = new();

    [Fact(DisplayName = "Проверка ошибка при подписке после утилизации")]
    public void Subscribe_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var eventBus = new GameLoopEventBus();
        eventBus.Dispose();

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            await Task.CompletedTask;
        }

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(
            () => eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>(
                "TestLoop",
                Handler));
    }

    [Fact(DisplayName = "Проверка публикация после утилизации вызывает исключение")]
    public async Task PublishAsync_AfterDispose_ShouldThrow()
    {
        // Arrange
        var eventBus = new GameLoopEventBus();
        eventBus.Dispose();
        var evt = _fixture.CreateTestEvent();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await eventBus.PublishAsync(evt));
    }

    [Fact(DisplayName = "Проверка синхронная публикация после утилизации вызывает исключение")]
    public void Publish_AfterDispose_ShouldThrow()
    {
        // Arrange
        var eventBus = new GameLoopEventBus();
        eventBus.Dispose();
        var evt = _fixture.CreateTestEvent();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => eventBus.Publish(evt));
    }

    [Fact(DisplayName = "Проверка утилизация очищает подписчиков")]
    public async Task Dispose_WithActiveSubscriptions_ClearsSubscribers()
    {
        // Arrange
        var eventBus = new GameLoopEventBus();
        var handlerCalled = false;

        async Task Handler(GameLoopEventBusFixture.TestGameLoopEvent evt, CancellationToken ct)
        {
            handlerCalled = true;
            await Task.CompletedTask;
        }

        eventBus.Subscribe<GameLoopEventBusFixture.TestGameLoopEvent>("TestLoop", Handler);
        var evt = _fixture.CreateTestEvent();

        // Act
        eventBus.Dispose();

        // Try to publish after dispose - should fail
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => eventBus.PublishAsync(evt));

        // Assert
        Assert.False(handlerCalled);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
