using Ais.GameEngine.Core.Internal.StateMachine;
using Ais.GameEngine.Core.Tests.Fixtures;
using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

namespace Ais.GameEngine.Core.Tests.StateMachine;

public sealed class GameStateMachineLifecycleTests : IDisposable
{
    private readonly GameStateMachineFixture _fixture;
    private readonly ILogger<GameStateMachine> _logger;

    public GameStateMachineLifecycleTests()
    {
        _fixture = new GameStateMachineFixture();
        _logger = NullLogger<GameStateMachine>.Instance;
        _fixture.SetupStateExecutorCalls();
    }

    [Fact(DisplayName = "Проверка запуск машины состояний с начальным состоянием")]
    public async Task StartAsync_WithInitialState_ChangesToInitialState()
    {
        // Arrange
        var initialState = _fixture.CreateMockState("InitialState");
        _fixture.SetupStateProvider<IGameState>(initialState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act
        await stateMachine.StartAsync<IGameState>(CancellationToken.None);

        // Assert
        Assert.Same(initialState, stateMachine.CurrentState);
        await _fixture.StateExecutor.Received(1)
            .EnterAsync(initialState, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка вызов Execute при запуске машины")]
    public async Task StartAsync_RunsExecutionLoop_CallsExecuteOnCurrentState()
    {
        // Arrange
        var initialState = _fixture.CreateMockState("InitialState");
        _fixture.SetupStateProvider<IGameState>(initialState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        var executeCallCount = 0;
        _fixture.StateExecutor.ExecuteAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(async x =>
            {
                executeCallCount++;
                if (executeCallCount > 2)
                {
                    await stateMachine.StopAsync();
                }
            });

        // Act
        await stateMachine.StartAsync<IGameState>(CancellationToken.None);
        await  Task.Delay(100);

        // Assert
        Assert.True(executeCallCount > 0);
        await _fixture.StateExecutor.Received(1)
            .EnterAsync(initialState, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка остановка машины состояний")]
    public async Task StopAsync_RunningMachine_StopsExecution()
    {
        // Arrange
        var initialState = _fixture.CreateMockState("InitialState");
        _fixture.SetupStateProvider<IGameState>(initialState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        var executeCallCount = 0;
        _fixture.StateExecutor.ExecuteAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(async x =>
            {
                executeCallCount++;
                if (executeCallCount > 1)
                {
                    await stateMachine.StopAsync();
                }
            });

        // Act
        await stateMachine.StartAsync<IGameState>(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        await _fixture.StateExecutor.Received(1)
            .ExitAsync(initialState, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка идемпотентность остановки уже остановленной машины")]
    public async Task StopAsync_AlreadyStopped_DoesNotThrow()
    {
        // Arrange
        var initialState = _fixture.CreateMockState("InitialState");
        _fixture.SetupStateProvider<IGameState>(initialState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act & Assert
        await stateMachine.StopAsync();
        await stateMachine.StopAsync();
    }

    [Fact(DisplayName = "Проверка идемпотентность StartAsync при уже запущенной машине")]
    public async Task StartAsync_AlreadyRunning_DoesNotRestart()
    {
        // Arrange
        var initialState = _fixture.CreateMockState("InitialState");
        _fixture.SetupStateProvider<IGameState>(initialState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        var executeCallCount = 0;
        _fixture.StateExecutor.ExecuteAsync(Arg.Any<IGameState>(), Arg.Any<CancellationToken>())
            .Returns(async x =>
            {
                executeCallCount++;
                if (executeCallCount > 2)
                {
                    await stateMachine.StopAsync();
                }
            });

        // Act
        var startTask1 = stateMachine.StartAsync<IGameState>(CancellationToken.None);
        var startTask2 = stateMachine.StartAsync<IGameState>(CancellationToken.None);

        await startTask1;
        await startTask2;

        // Assert
        await _fixture.StateExecutor.Received(1)
            .EnterAsync(initialState, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка ошибка при StartAsync когда контекст null")]
    public async Task StartAsync_ContextIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        _fixture.ContextAccessor.CurrentContext.Returns((GameContext)null!);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => stateMachine.StartAsync<IGameState>());
    }

    [Fact(DisplayName = "Проверка ошибка при StopAsync когда контекст null")]
    public async Task StopAsync_ContextIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        _fixture.ContextAccessor.CurrentContext.Returns((GameContext)null!);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => stateMachine.StopAsync());
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
