namespace Ais.ECS.Abstractions.Queries;

/// <summary>
/// Поставщик запросов
/// </summary>
public interface IQueryProvider
{
    /// <summary>
    /// Фабрика запросов
    /// </summary>
    IQueryBuilder QueryBuilder { get; }

    /// <summary>
    /// Выпонить запрос
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <returns>Результат выполнения запроса</returns>
    QueryResult Execute(IQuery query);
}