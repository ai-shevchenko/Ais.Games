namespace Ais.ECS.Abstractions.Entities;

/// <summary>
/// Реестр сущностей
/// </summary>
public interface IEntityRegistry
{
    /// <summary>
    /// Получить список всех сущностей
    /// </summary>
    /// <returns></returns>
    ReadOnlySpan<IEntity> GetAllEntities();

    /// <summary>
    /// Получить список сущностей по идентификатору
    /// </summary>
    /// <param name="entityId">Идентификатор сущности</param>
    /// <returns>Сущность</returns>
    ReadOnlySpan<IEntity> GetEntities(EntityId entityId);
}