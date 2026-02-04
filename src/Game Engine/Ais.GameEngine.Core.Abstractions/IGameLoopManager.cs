namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Поставщик игровых циклов
/// </summary>
public interface IGameLoopManager
{
    /// <summary>
    ///     Список игровых циклов
    /// </summary>
    IReadOnlyList<IGameLoop> GameLoops { get; }

    /// <summary>
    ///     Получить игровой цикл
    /// </summary>
    /// <param name="name">Наименование игрового цикла</param>
    /// <returns>Игровой цикл</returns>
    IGameLoop GetGameLoop(string name);

    /// <summary>
    ///     Получить или создать новый игровой цикл
    /// </summary>
    /// <param name="name">Наименование игрового цикла</param>
    /// <param name="configure">Настройка контекста выполнения цикла</param>
    /// <returns>Игровой цикл</returns>
    IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings>? configure = null);

    /// <summary>
    ///     Определить наличие игрового цикла внутри системы
    /// </summary>
    /// <param name="name">Наименование игрового цикла</param>
    /// <returns>Наличие игрового цикла в системе</returns>
    bool HasGameLoop(string name);
}
