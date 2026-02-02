namespace Ais.ECS.Abstractions.Queries;

/// <summary>
///     Фабрика запросов
/// </summary>
public interface IQueryBuilder
{
    /// <summary>
    ///     Создать запрос
    /// </summary>
    /// <returns>Запрос</returns>
    IQuery CreateQuery();
}
