using Ais.ECS.Abstractions.Components;
using Ais.ECS.Abstractions.Queries;
using IQueryProvider = Ais.ECS.Abstractions.Queries.IQueryProvider;

namespace Ais.ECS.Queries;

internal sealed class Query : IQuery
{
    private readonly IQueryProvider _provider;
    private readonly List<Type> _withComponents = [];
    private readonly List<Type> _withoutComponents = [];

    public Query(IQueryProvider provider)
    {
        _provider = provider;
    }

    public IReadOnlyList<Type> WithComponents => _withComponents;
    
    public IReadOnlyList<Type> WithoutComponents => _withoutComponents;

    public IQuery With<T>() 
        where T : IComponent
    {
        _withComponents.Add(typeof(T));
        return this;
    }

    public IQuery Without<T>() 
        where T : IComponent
    {
        _withoutComponents.Add(typeof(T));
        return this;
    }

    public QueryResult GetResult()
    {
        return _provider.Execute(this);
    }
}