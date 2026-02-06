using System.ComponentModel.DataAnnotations;

namespace Ais.GameEngine.Core.Settings;

public class GameTimerSettings
{
    [Range(0, 1f)] public float FixedDeltaTime { get; set; } = 0.016f;

    [Range(0, 1f)] public float MaxFrameTime { get; set; } = 0.25f;

    [Range(24, float.MaxValue)] public float TargetFrameRate { get; set; } = 60;

    [Range(1, int.MaxValue)] public int MaxFixedUpdateIteration { get; set; } = 10;
}
