using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Источник хуков игрового цикла
/// </summary>
public interface IHooksSource : IDisposable
{
    /// <summary>
    /// Получить список хуков
    /// </summary>
    /// <typeparam name="T">Тип хука</typeparam>
    /// <param name="enabledOnly">Получить только включенные хуки</param>
    /// <returns>Список хуков</returns>
    IReadOnlyList<T> GetHooks<T>(bool enabledOnly = false)
        where T : class, IHook;

    /// <summary>
    /// Получить хук
    /// </summary>
    /// <typeparam name="T">Тип хука</typeparam>
    /// <returns>Хук</returns>
    T GetHook<T>()
        where T : class, IHook;

    /// <summary>
    /// Добавить хук
    /// </summary>
    /// <typeparam name="T">Тип хука</typeparam>
    void AddHook<T>()
        where T : class, IHook;

    /// <summary>
    /// Добавить хук
    /// </summary>
    /// <typeparam name="T">Тип хука</typeparam>
    /// <param name="order">Порядок хука</param>
    void AddHook<T>(int order)
        where T : class, IHook;
}
