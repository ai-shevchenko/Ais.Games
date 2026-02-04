namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Механизм получения контекста игрового цикла, в рамках игровых систем запущенных внутри захваченного игрового цикла
/// </summary>
public interface IGameLoopContextAccessor
{
    /// <summary>
    ///     Контекст игрового цикла
    /// </summary>
    GameLoopContext? CurrentContext { get; set; }
}
