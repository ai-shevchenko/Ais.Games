namespace Ais.GameEngine.Core.Settings;

/// <summary>
///     Игровые настройки
/// </summary>
public class GameEngineSettings
{
    /// <summary>
    ///     Настройки системы игрового времени
    /// </summary>
    public GameTimerSettings TimerSettings { get; set; } = new();
}
