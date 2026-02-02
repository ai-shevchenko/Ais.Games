using System.ComponentModel.DataAnnotations;

namespace Ais.GameEngine.Core.TimeSystem;

/// <summary>
///     ��������� �������� �������
/// </summary>
public class GameTimerSettings
{
    /// <summary>
    ///     ������������� ��� �������
    /// </summary>
    [Range(0, 1f)]
    public float FixedDeltaTime { get; set; } = 0.016f;

    /// <summary>
    ///     ������������ ������ �����
    /// </summary>
    [Range(0, 1f)]
    public float MaxFrameTime { get; set; } = 0.25f;

    /// <summary>
    ///     ������� ������� ������
    /// </summary>
    [Range(24, float.MaxValue)]
    public float TargetFrameRate { get; set; } = 60;

    /// <summary>
    ///     ������������ ���-�� ������������� ���������� �� ����
    /// </summary>
    [Range(1, int.MaxValue)]
    public int MaxFixedUpdateIteration { get; set; } = 10;
}
