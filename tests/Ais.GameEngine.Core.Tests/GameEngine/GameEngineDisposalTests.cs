using Ais.GameEngine.Core.Tests.Fixtures;

namespace Ais.GameEngine.Core.Tests.GameEngine;

public sealed class GameEngineDisposalTests : IDisposable
{
    private readonly GameLoopFactoryFixture _fixture = new();

    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Fact(DisplayName = "Проверка ошибка при использовании двигателя после утилизации")]
    public void CreateGameLoop_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => engine.CreateGameLoop("TestLoop"));
    }

    [Fact(DisplayName = "Проверка ошибка при старте двигателя после утилизации")]
    public async Task StartAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => engine.StartAsync());
    }

    [Fact(DisplayName = "Проверка ошибка при остановке двигателя после утилизации")]
    public async Task StopAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => engine.StopAsync());
    }

    [Fact(DisplayName = "Проверка двойная утилизация не вызывает исключение")]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Act & Assert
        engine.Dispose();
        engine.Dispose();
    }
}
