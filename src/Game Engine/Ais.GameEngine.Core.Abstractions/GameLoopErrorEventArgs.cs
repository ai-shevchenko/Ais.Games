namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Аргументы события ошибки игрового цикла.
/// </summary>
public class GameLoopErrorEventArgs : GameLoopEventArgs
{
    /// <summary>
    /// Исключение, вызвавшее ошибку.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
