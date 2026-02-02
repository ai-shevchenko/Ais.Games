using Ais.ECS.Abstractions.Entities;

namespace Ais.ECS.Abstractions.Queries;

/// <summary>
///     Результат выполнения запроса
/// </summary>
public readonly ref struct QueryResult
{
    /// <summary>
    ///     Список сущностей удовлетворяющих фильтрам запроса
    /// </summary>
    public required ReadOnlySpan<IEntity> Entities { get; init; }

    /// <summary>
    ///     Кол-во полученных сущностей
    /// </summary>
    public required int Count { get; init; }
}
