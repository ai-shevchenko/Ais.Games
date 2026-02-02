using Ais.ECS.Abstractions.Worlds;

namespace Ais.ECS.Abstractions.Systems;

/// <summary>
///     Контекст системы
/// </summary>
/// <param name="World">Мир</param>
public record SystemContext(
    IWorld World);
