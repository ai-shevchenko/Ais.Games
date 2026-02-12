using Ais.GameEngine.Core.Abstractions;
using NSubstitute;

namespace Ais.GameEngine.Core.Tests.Fixtures;

public sealed class GameLoopFactoryFixture : IDisposable
{
    public IGameLoopFactory Factory { get; } = Substitute.For<IGameLoopFactory>();
    public List<GameLoopScope> CreatedScopes { get; } = [];

    public GameLoopScope CreateMockScope(string loopName)
    {
        var gameLoop = Substitute.For<IGameLoop>();
        gameLoop.Name.Returns(loopName);
        gameLoop.State.Returns(GameLoopState.Stopped);
        gameLoop.IsRunning.Returns(false);
        gameLoop.IsPaused.Returns(false);

        var scopeDisposable = Substitute.For<IDisposable>();
        var scope = new GameLoopScope(loopName, gameLoop, scopeDisposable);
        CreatedScopes.Add(scope);

        return scope;
    }

    public void SetupFactoryToCreateLoop(string loopName)
    {
        var scope = CreateMockScope(loopName);
        Factory.Create(loopName, Arg.Any<Action<GameLoopBuilderSettings>?>()).Returns(scope);
    }

    public void Dispose()
    {
        foreach (var scope in CreatedScopes)
        {
            scope.Dispose();
        }

        CreatedScopes.Clear();
    }
}
