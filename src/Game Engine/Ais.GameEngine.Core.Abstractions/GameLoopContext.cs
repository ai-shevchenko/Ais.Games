namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Контекст игрового цикла
/// </summary>
public class GameLoopContext
{
    /// <summary>
    ///     Наименование игрового цикла
    /// </summary>
    public required string LoopName { get; init; }

    /// <summary>
    ///     Поставщик хуков
    /// </summary>
    public required IHooksProvider Hooks { get; init; }

    /// <summary>
    ///     Поставщик состояний
    /// </summary>
    public required IGameLoopStateProvider StatesProvider { get; init; }

    /// <summary>
    ///     Текущее состояние
    /// </summary>
    public IGameLoopState? CurrentState { get; set; }

    /// <summary>
    ///     Игровые данные
    /// </summary>
    public Dictionary<string, object?> GameData { get; } = [];
}
