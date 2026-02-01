using Ais.ECS.Abstractions.Queries;
using IQueryProvider = Ais.ECS.Abstractions.Queries.IQueryProvider;

namespace Ais.ECS.Queries;

internal sealed class QueryBuilder : IQueryBuilder
{
    private readonly IQueryProvider _provider;

    public QueryBuilder(IQueryProvider provider)
    {
        _provider = provider;
    }

    public IQuery CreateQuery()
    {
        return new Query(_provider);
    }
}
