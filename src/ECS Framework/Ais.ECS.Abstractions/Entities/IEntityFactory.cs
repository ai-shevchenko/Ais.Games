namespace Ais.ECS.Abstractions.Entities;

/// <summary>
///     ������� ���������
/// </summary>
public interface IEntityFactory
{
    /// <summary>
    ///     ������� ��������
    /// </summary>
    /// <returns>��������</returns>
    IEntity CreateEntity();

    /// <summary>
    ///     ���������� ��������
    /// </summary>
    /// <param name="entity">��������</param>
    void DestroyEntity(IEntity entity);
}
