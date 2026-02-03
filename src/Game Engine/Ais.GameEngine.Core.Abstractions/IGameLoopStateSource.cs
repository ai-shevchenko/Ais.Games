namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Источник состояний игрового цикла
/// </summary>
public interface IGameLoopStateSource
{
    /// <summary>
    ///     Добавить новое состояние
    /// </summary>
    /// <typeparam name="T">Тип состояния</typeparam>
    void RegisterState<T>()
        where T : IGameLoopState;
}
