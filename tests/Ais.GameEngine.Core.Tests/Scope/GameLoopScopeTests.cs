using Ais.GameEngine.Core.Abstractions;
using NSubstitute;

namespace Ais.GameEngine.Core.Tests.Scope;

public sealed class GameLoopScopeTests
{
    [Fact(DisplayName = "Проверка получение имени цикла из области")]
    public void GameLoopScope_Constructor_StoresName()
    {
        // Arrange
        var name = "TestLoop";
        var gameLoop = Substitute.For<IGameLoop>();
        var disposable = Substitute.For<IDisposable>();

        // Act
        var scope = new GameLoopScope(name, gameLoop, disposable);

        // Assert
        Assert.Equal(name, scope.Name);
    }

    [Fact(DisplayName = "Проверка получение игрового цикла из области")]
    public void GameLoopScope_Constructor_StoresGameLoop()
    {
        // Arrange
        var name = "TestLoop";
        var gameLoop = Substitute.For<IGameLoop>();
        var disposable = Substitute.For<IDisposable>();

        // Act
        var scope = new GameLoopScope(name, gameLoop, disposable);

        // Assert
        Assert.Same(gameLoop, scope.GameLoop);
    }

    [Fact(DisplayName = "Проверка получение disposable из области")]
    public void GameLoopScope_Constructor_StoresScope()
    {
        // Arrange
        var name = "TestLoop";
        var gameLoop = Substitute.For<IGameLoop>();
        var disposable = Substitute.For<IDisposable>();

        // Act
        var scope = new GameLoopScope(name, gameLoop, disposable);

        // Assert
        Assert.Same(disposable, scope.Scope);
    }

    [Fact(DisplayName = "Проверка утилизация области вызывает Dispose игрового цикла")]
    public void GameLoopScope_Dispose_DisposesGameLoop()
    {
        // Arrange
        var name = "TestLoop";
        var gameLoop = Substitute.For<IGameLoop>();
        var disposable = Substitute.For<IDisposable>();
        var scope = new GameLoopScope(name, gameLoop, disposable);

        // Act
        scope.Dispose();

        // Assert
        gameLoop.Received(1).Dispose();
    }

    [Fact(DisplayName = "Проверка утилизация области вызывает Dispose для scope")]
    public void GameLoopScope_Dispose_DisposesScope()
    {
        // Arrange
        var name = "TestLoop";
        var gameLoop = Substitute.For<IGameLoop>();
        var disposable = Substitute.For<IDisposable>();
        var scope = new GameLoopScope(name, gameLoop, disposable);

        // Act
        scope.Dispose();

        // Assert
        disposable.Received(1).Dispose();
    }

    [Fact(DisplayName = "Проверка утилизация области вызывает оба Dispose в правильном порядке")]
    public void GameLoopScope_Dispose_DisposesInOrder()
    {
        // Arrange
        var name = "TestLoop";
        var callOrder = new List<string>();
        var gameLoop = Substitute.For<IGameLoop>();
        gameLoop.When(x => x.Dispose()).Do(_ => callOrder.Add("GameLoop"));

        var disposable = Substitute.For<IDisposable>();
        disposable.When(x => x.Dispose()).Do(_ => callOrder.Add("Scope"));

        var scope = new GameLoopScope(name, gameLoop, disposable);

        // Act
        scope.Dispose();

        // Assert
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("GameLoop", callOrder[0]);
        Assert.Equal("Scope", callOrder[1]);
    }
}
