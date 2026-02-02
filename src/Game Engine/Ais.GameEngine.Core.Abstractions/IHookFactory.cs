using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Фабрика хуков игрового цикла
/// </summary>
public interface IHookFactory
{
    /// <summary>
    ///     Создать хук
    /// </summary>
    /// <typeparam name="T">Тип хука</typeparam>
    /// <returns>Хук</returns>
    T CreateHook<T>()
        where T : class, IHook;

    /// <summary>
    ///     Создать хук
    /// </summary>
    /// <param name="hookType">Тип хука</param>
    /// <returns>Хук</returns>
    IHook CreateHook(Type hookType);
}
