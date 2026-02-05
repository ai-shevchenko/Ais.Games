namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Фабрика игрового цикла
/// </summary>
public interface IGameLoopFactory
{
    /// <summary>
    ///     Создать игровой цикл
    /// </summary>
    /// <param name="name">Наименование игрового цикла</param>
    /// <param name="configure">Настройки игрового цикла</param>
    /// <returns>Игровой цикл</returns>
    GameLoopScope Create(string name, Action<GameLoopBuilderSettings>? configure = null);
}
