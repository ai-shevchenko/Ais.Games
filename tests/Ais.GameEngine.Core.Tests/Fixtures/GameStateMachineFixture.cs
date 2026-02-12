using Ais.GameEngine.StateMachine.Abstractions;

using NSubstitute;

namespace Ais.GameEngine.Core.Tests.Fixtures;

public sealed class GameStateMachineFixture : IDisposable
{
    public GameStateMachineFixture()
    {
        StateProvider = Substitute.For<IGameStateProvider>();
        ContextAccessor = Substitute.For<IGameContextAccessor>();
        StateExecutor = Substitute.For<IGameStateExecutor>();
        GameContext = new GameContext { LoopName = "TestLoop" };

        ContextAccessor.CurrentContext.Returns(GameContext);
    }

    public IGameStateProvider StateProvider { get; }
    public IGameContextAccessor ContextAccessor { get; }
    public IGameStateExecutor StateExecutor { get; }
    public GameContext GameContext { get; }

    public void Dispose()
    {
    }

    public IGameState CreateMockState(string stateName = "TestState")
    {
        return Substitute.For<IGameState>();
    }

    public void SetupStateProvider<T>(T state) where T : IGameState
    {
        StateProvider.GetState<T>().Returns(state);
    }

    public void SetupStateExecutorCalls()
    {
        StateExecutor.EnterAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        StateExecutor.ExecuteAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        StateExecutor.ExitAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }
}
