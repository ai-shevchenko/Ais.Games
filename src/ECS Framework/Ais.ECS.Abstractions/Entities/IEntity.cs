namespace Ais.ECS.Abstractions.Entities;


/// <summary>
/// Сущность
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    EntityId Id { get; }

    /// <summary>
    /// Версия сущности
    /// </summary>
    int Version { get; }
}