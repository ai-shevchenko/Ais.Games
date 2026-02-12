namespace Ais.GameEngine.DependencyInjection.Abstractions;

/// <summary>
/// Абстракция для DI контейнера, изолирующая зависимость от конкретно фреймворка (Autofac, Unity и т.д.).
/// Позволяет легко заменить DI реализацию без изменения остального кода.
/// </summary>
public interface IServiceContainer : IDisposable
{
    /// <summary>
    /// Разрешить экземпляр сервиса по заданному типу.
    /// </summary>
    /// <typeparam name="T">Тип сервиса для разрешения.</typeparam>
    /// <returns>Экземпляр сервиса.</returns>
    /// <exception cref="InvalidOperationException">Если сервис не зарегистрирован.</exception>
    T Resolve<T>() where T : notnull;

    /// <summary>
    /// Попытаться разрешить сервис, возвращая успешность операции.
    /// </summary>
    /// <typeparam name="T">Тип сервиса для разрешения.</typeparam>
    /// <param name="instance">Разрешённый экземпляр сервиса, если успешно.</param>
    /// <returns>true, если сервис успешно разрешен; иначе false.</returns>
    bool TryResolve<T>(out T instance) where T : notnull;
}
