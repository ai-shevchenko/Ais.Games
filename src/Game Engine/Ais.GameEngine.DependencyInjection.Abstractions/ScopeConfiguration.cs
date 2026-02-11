using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Конфигурация области видимости в DI контейнере.
/// </summary>
public class ScopeConfiguration
{
    /// <summary>
    /// Имя области видимости.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Дополнительные сервисы, регистрируемые в рамках области видимости.
    /// </summary>
    public Action<IServiceCollection>? ConfigureServices { get; set; }
}
