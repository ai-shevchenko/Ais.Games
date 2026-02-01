namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Фабрика состояний игрового цикла
/// </summary>
public interface IGameLoopStateFactory
{
   /// <summary>
   /// Создать состояние
   /// </summary>
   /// <typeparam name="T">Тип состояния</typeparam>
   /// <returns>Состояние игрового цикла</returns>
    T CreateState<T>() 
        where T : IGameLoopState;

    /// <summary>
    /// Создать состояние
    /// </summary>
    /// <param name="stateType">Тип состояния</param>
    /// <returns>Состояние игрового цикла</returns>
    IGameLoopState CreateState(Type stateType);
}