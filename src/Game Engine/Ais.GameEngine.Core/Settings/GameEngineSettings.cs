using Ais.GameEngine.Core.TimeSystem;

namespace Ais.GameEngine.Core.Settings;

/// <summary>
///     Игровые настройки
/// </summary>
public class GameEngineSettings
{
    public GameTimerSettings TimerSettings { get; set; } = new();
}
