using Ais.GameEngine.Core.Internal.StateMachine;
using Ais.GameEngine.Core.Tests.Fixtures;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

namespace Ais.GameEngine.Core.Tests.StateMachine;

public sealed class GameStateMachineDisposalTests : IDisposable
{
    private readonly GameStateMachineFixture _fixture;
    private readonly ILogger<GameStateMachine> _logger;

    public GameStateMachineDisposalTests()
    {
        _fixture = new GameStateMachineFixture();
        _logger = NullLogger<GameStateMachine>.Instance;
        _fixture.SetupStateExecutorCalls();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Fact(DisplayName = "Проверка ошибка при ChangeStateAsync после утилизации")]
    public async Task ChangeStateAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        stateMachine.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => stateMachine.ChangeStateAsync<IGameState>());
    }

    [Fact(DisplayName = "Проверка ошибка при StartAsync после утилизации")]
    public async Task StartAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        stateMachine.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => stateMachine.StartAsync<IGameState>());
    }

    [Fact(DisplayName = "Проверка ошибка при StopAsync после утилизации")]
    public async Task StopAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        stateMachine.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => stateMachine.StopAsync());
    }

    [Fact(DisplayName = "Проверка утилизация машины останавливает выполнение")]
    public async Task Dispose_RunningMachine_StopsExecution()
    {
        // Arrange
        var initialState = _fixture.CreateMockState("InitialState");
        _fixture.SetupStateProvider(initialState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        _fixture.StateExecutor.ExecuteAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(Task.Delay(10));

        var startTask = stateMachine.StartAsync<IGameState>(CancellationToken.None);

        // Act
        stateMachine.Dispose();

        // Assert
        await startTask;
        await _fixture.StateExecutor.Received(1)
            .ExitAsync(initialState, Arg.Any<CancellationToken>());
    }
}
