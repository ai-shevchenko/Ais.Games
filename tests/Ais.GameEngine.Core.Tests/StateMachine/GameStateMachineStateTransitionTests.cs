using Ais.GameEngine.Core.Internal.StateMachine;
using Ais.GameEngine.Core.Tests.Fixtures;
using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

namespace Ais.GameEngine.Core.Tests.StateMachine;

public sealed class GameStateMachineStateTransitionTests : IDisposable
{
    private readonly GameStateMachineFixture _fixture;
    private readonly ILogger<GameStateMachine> _logger;

    public GameStateMachineStateTransitionTests()
    {
        _fixture = new GameStateMachineFixture();
        _logger = NullLogger<GameStateMachine>.Instance;
        _fixture.SetupStateExecutorCalls();
    }

    [Fact(DisplayName = "Проверка начальное состояние машины равно null")]
    public void Constructor_NoArguments_CurrentStateIsNull()
    {
        // Arrange & Act
        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Assert
        Assert.Null(stateMachine.CurrentState);
    }

    [Fact(DisplayName = "Проверка смена состояния на новое")]
    public async Task ChangeStateAsync_WithNewState_ChangesCurrentState()
    {
        // Arrange
        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        var newState = _fixture.CreateMockState("NewState");
        _fixture.SetupStateProvider(newState);

        // Act
        await stateMachine.ChangeStateAsync<IGameState>();

        // Assert
        Assert.Same(newState, stateMachine.CurrentState);
        Assert.Same(newState, _fixture.GameContext.CurrentState);
    }

    [Fact(DisplayName = "Проверка вызов Exit для предыдущего состояния при смене")]
    public async Task ChangeStateAsync_WithExistingState_CallsExitOnPreviousState()
    {
        // Arrange
        var oldState = _fixture.CreateMockState("OldState");
        var newState = _fixture.CreateMockState("NewState");

        _fixture.GameContext.CurrentState = oldState;
        _fixture.SetupStateProvider(newState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act
        await stateMachine.ChangeStateAsync<IGameState>();

        // Assert
        await _fixture.StateExecutor.Received(1)
            .ExitAsync(oldState, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка вызов Enter для нового состояния при смене")]
    public async Task ChangeStateAsync_WithNewState_CallsEnterOnNewState()
    {
        // Arrange
        var newState = _fixture.CreateMockState("NewState");
        _fixture.SetupStateProvider<IGameState>(newState);

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act
        await stateMachine.ChangeStateAsync<IGameState>();

        // Assert
        await _fixture.StateExecutor.Received(1)
            .EnterAsync(newState, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Проверка восстановление предыдущего состояния при ошибке Enter")]
    public async Task ChangeStateAsync_EnterThrows_RestoresPreviousState()
    {
        // Arrange
        var oldState = _fixture.CreateMockState("OldState");
        var newState = _fixture.CreateMockState("NewState");

        _fixture.GameContext.CurrentState = oldState;
        _fixture.SetupStateProvider<IGameState>(newState);

        _fixture.StateExecutor.EnterAsync(newState, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Enter failed")));

        var stateMachine = new GameStateMachine(
            _fixture.StateProvider,
            _fixture.ContextAccessor,
            _logger,
            _fixture.StateExecutor);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StateTransitionException>(
            () => stateMachine.ChangeStateAsync<IGameState>());

        Assert.Same(oldState, stateMachine.CurrentState);
        Assert.Contains("Failed to enter state", exception.Message);
    }

    [Fact(DisplayName = "Проверка ошибка при смене состояния когда контекст null")]
    public async Task ChangeStateAsync_ContextIsNull_ThrowsArgumentNullException()
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
            () => stateMachine.ChangeStateAsync<IGameState>());
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
