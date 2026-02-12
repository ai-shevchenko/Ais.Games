namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Аргументы события игрового цикла.
/// </summary>
public class GameLoopEventArgs : EventArgs
{
    /// <summary>
    /// Имя игрового цикла.
    /// </summary>
    public string LoopName { get; set; } = string.Empty;

    /// <summary>
    /// Временная метка события.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
