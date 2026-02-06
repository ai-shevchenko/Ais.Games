namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Поставщик состояний игрового цикла
/// </summary>
public interface IGameStateProvider
{
    /// <summary>
    ///     Получить состояние
    /// </summary>
    /// <typeparam name="T">Тип состояния</typeparam>
    /// <returns>Состояние игрового цикла</returns>
    T GetState<T>()
        where T : IGameState;

    /// <summary>
    ///     Получить состояние
    /// </summary>
    /// <param name="stateType">Тип состояния</param>
    /// <returns>Состояние игрового цикла</returns>
    IGameState GetState(Type stateType);
}
