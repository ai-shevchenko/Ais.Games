using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Modules.Abstractions;

/// <summary>
///     Модуль игрового движка
/// </summary>
public abstract class GameEngineModule
{
    /// <summary>
    ///     Конфигурация игровых сервисов
    /// </summary>
    /// <param name="gameServices">Список игровых сервисов</param>
    /// <param name="configuration">Конфигурация игры</param>
    public abstract void ConfigureGameServices(IServiceCollection gameServices, IConfiguration configuration);
}
