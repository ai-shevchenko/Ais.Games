namespace Ais.GameEngine.DependencyInjection.Abstractions;

/// <summary>
/// Абстракция для создания областей видимости в DI контейнере.
/// Используется для создания изолированных контекстов сервисов на уровне игрового цикла.
/// </summary>
public interface IServiceScopeFactory
{
    /// <summary>
    /// Создать новую область видимости с отдельным контейнером сервисов.
    /// </summary>
    /// <param name="scopeName">Уникальное имя области видимости (для отладки и идентификации).</param>
    /// <param name="configure">Дополнительная конфигурация сервисов в рамках области.</param>
    /// <returns>Контейнер для новой области видимости.</returns>
    IServiceContainer CreateScope(string scopeName, Action<ScopeConfiguration>? configure = null);
}
