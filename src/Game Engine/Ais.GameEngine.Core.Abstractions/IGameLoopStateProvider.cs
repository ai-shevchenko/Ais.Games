namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Поставщик состояний игрового цикла
/// </summary>
public interface IGameLoopStateProvider
{
    /// <summary>
    ///     Получить состояние
    /// </summary>
    /// <typeparam name="T">Тип состояния</typeparam>
    /// <returns>Состояние игрового цикла</returns>
    T GetState<T>()
        where T : IGameLoopState;

    /// <summary>
    ///     Получить состояние
    /// </summary>
    /// <param name="stateType">Тип состояния</param>
    /// <returns>Состояние игрового цикла</returns>
    IGameLoopState GetState(Type stateType);
}
