using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Конфигуратор игрового движка
/// </summary>
public interface IGameEngineBuilder
{
    /// <summary>
    ///     Сконфигурировать игровые сервисы
    /// </summary>
    /// <param name="configure">Функция конфигурации</param>
    void ConfigureGameServices(Action<GameEngineBuilderContext, IServiceCollection> configure);

    /// <summary>
    ///     Сконфигурировать игровые настройки
    /// </summary>
    /// <param name="configure">Функция конфигурации</param>
    void ConfigureGameConfiguration(Action<IConfigurationBuilder> configure);

    /// <summary>
    ///     Сконфигурировать логирование игры
    /// </summary>
    /// <param name="configure">Функция конфигурации</param>
    void ConfigureGameLogging(Action<GameEngineBuilderContext, ILoggingBuilder> configure);

    /// <summary>
    ///     Добавить обогатитель модулей
    /// </summary>
    /// <param name="enricher">Экземпляр обогатителя</param>
    void AddModuleEnricher(IModuleEnricher enricher);

    /// <summary>
    ///     Собрать игровой движок
    /// </summary>
    /// <returns>Игровой движок</returns>
    IGameEngine Build();
}
