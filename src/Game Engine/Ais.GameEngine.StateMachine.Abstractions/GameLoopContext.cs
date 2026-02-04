namespace Ais.GameEngine.StateMachine.Abstractions;

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
    ///     Текущее состояние
    /// </summary>
    public IGameLoopState? CurrentState { get; set; }

    /// <summary>
    ///     Игровые данные
    /// </summary>
    public Dictionary<string, object?> GameData { get; } = [];
}
