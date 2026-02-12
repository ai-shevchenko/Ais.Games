using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Tests.Fixtures;

namespace Ais.GameEngine.Core.Tests.GameEngine;

public sealed class GameEngineGameLoopManagementTests : IDisposable
{
    private readonly GameLoopFactoryFixture _fixture = new();

    [Fact(DisplayName = "Проверка создание игрового цикла")]
    public void CreateGameLoop_WithValidName_CreatesGameLoop()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("TestLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Act
        var gameLoop = engine.CreateGameLoop("TestLoop");

        // Assert
        Assert.NotNull(gameLoop);
        Assert.True(engine.HasGameLoop("TestLoop"));
    }

    [Fact(DisplayName = "Проверка получение уже созданного игрового цикла")]
    public void GetGameLoop_ExistingLoop_ReturnsGameLoop()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("TestLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var createdLoop = engine.CreateGameLoop("TestLoop");

        // Act
        var retrievedLoop = engine.GetGameLoop("TestLoop");

        // Assert
        Assert.Same(createdLoop, retrievedLoop);
    }

    [Fact(DisplayName = "Проверка ошибка при получении несуществующего цикла")]
    public void GetGameLoop_NonExistentLoop_ThrowsKeyNotFoundException()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => engine.GetGameLoop("NonExistent"));
        Assert.Contains("NonExistent", exception.Message);
    }

    [Fact(DisplayName = "Проверка TryGetGameLoop находит существующий цикл")]
    public void TryGetGameLoop_ExistingLoop_ReturnsTrueAndGameLoop()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("TestLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        var createdLoop = engine.CreateGameLoop("TestLoop");

        // Act
        var result = engine.TryGetGameLoop("TestLoop", out var retrievedLoop);

        // Assert
        Assert.True(result);
        Assert.Same(createdLoop, retrievedLoop);
    }

    [Fact(DisplayName = "Проверка TryGetGameLoop возвращает false для несуществующего цикла")]
    public void TryGetGameLoop_NonExistentLoop_ReturnsFalse()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Act
        var result = engine.TryGetGameLoop("NonExistent", out var gameLoop);

        // Assert
        Assert.False(result);
        Assert.Null(gameLoop);
    }

    [Fact(DisplayName = "Проверка ошибка при создании цикла с дублирующимся именем")]
    public void CreateGameLoop_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("TestLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.CreateGameLoop("TestLoop");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => engine.CreateGameLoop("TestLoop"));
        Assert.Contains("Loop already exists", exception.Message);
    }

    [Fact(DisplayName = "Проверка HasGameLoop возвращает true для существующего цикла")]
    public void HasGameLoop_ExistingLoop_ReturnsTrue()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("TestLoop");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.CreateGameLoop("TestLoop");

        // Act
        var result = engine.HasGameLoop("TestLoop");

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "Проверка HasGameLoop возвращает false для несуществующего цикла")]
    public void HasGameLoop_NonExistentLoop_ReturnsFalse()
    {
        // Arrange
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);

        // Act
        var result = engine.HasGameLoop("NonExistent");

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "Проверка получение списка всех игровых циклов")]
    public void GameLoops_MultipleLoopsCreated_ReturnsAllLoops()
    {
        // Arrange
        _fixture.SetupFactoryToCreateLoop("Loop1");
        _fixture.SetupFactoryToCreateLoop("Loop2");
        using var engine = new Internal.GameLoop.GameEngine(_fixture.Factory);
        engine.CreateGameLoop("Loop1");
        engine.CreateGameLoop("Loop2");

        // Act
        var gameLoops = engine.GameLoops;

        // Assert
        Assert.Equal(2, gameLoops.Count);
        Assert.Contains(gameLoops, gl => gl.Name == "Loop1");
        Assert.Contains(gameLoops, gl => gl.Name == "Loop2");
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
