namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
/// Трекер игрового состояния
/// </summary>
public interface IGameStateTracker
{
    /// <summary>
    ///     Текущее состояние
    /// </summary>
    IGameState? CurrentState { get; }
}