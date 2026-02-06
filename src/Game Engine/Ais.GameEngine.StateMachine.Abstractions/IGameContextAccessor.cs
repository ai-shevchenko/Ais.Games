namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Механизм получения контекста игрового цикла, в рамках игровых систем запущенных внутри захваченного игрового цикла
/// </summary>
public interface IGameContextAccessor
{
    /// <summary>
    ///     Контекст игрового цикла
    /// </summary>
    GameContext? CurrentContext { get; set; }
}
