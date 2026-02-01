using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Queries;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using IQueryProvider = Ais.ECS.Abstractions.Queries.IQueryProvider;

namespace Ais.ECS.Queries;

internal sealed class QueryProvider : IQueryProvider
{
    private readonly IWorld _world;
    
    public QueryProvider(IWorld world)
    {
        _world = world;
        QueryBuilder = new QueryBuilder(this);
    }

    public IQueryBuilder QueryBuilder { get; }

    public QueryResult Execute(IQuery query)
    {
        var with = new ComponentMask(query.WithComponents);
        var without = new ComponentMask(query.WithoutComponents);
        
        var entities = new List<IEntity>();
        foreach (var entity in _world.GetAllEntities())
        {
            var components = entity.GetComponents(_world);
            var mask = new ComponentMask(components.Select(c => c.GetType()));

            if (!mask.Contains(with))
            {
                continue;
            }

            if (mask.Intersects(without))
            {
                continue;
            }
            
            entities.Add(entity);
        }

        var result = new IEntity[entities.Count];
        entities.CopyTo(result);
        entities.Clear();
        
        return new QueryResult
        {
            Entities = result.AsSpan(),
            Count = result.Length
        };
    }

    private readonly struct ComponentMask : IEquatable<ComponentMask>
    {
        private readonly ulong[] _bits;
        private readonly int _hash;

        public ComponentMask(IEnumerable<Type> types)
        {
            var bits = new HashSet<int>();
            foreach (var type in types)
            {
                bits.Add(type.GetHashCode() % 256);
            }

            var max = bits.Count > 0 ? bits.Max() : 0;
            _bits = new ulong[(max / 64) + 1];

            foreach (var bit in bits)
            {
                var element = bit / 64;
                var offset = bit % 64;
                _bits[element] |= 1UL << offset;
            }

            var hash = 17;
            foreach (var bit in _bits)
            {
                hash = hash * 31 + bit.GetHashCode();
            }
            _hash = hash;
        }

        public bool Contains(in ComponentMask other)
        {
            if (other._bits.Length > _bits.Length)
            {
                return false;
            }

            for (var i = 0; i < other._bits.Length; i++)
            {
                if ((other._bits[i] & ~_bits[i]) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Intersects(in ComponentMask other)
        {
            var minLength = Math.Min(_bits.Length, other._bits.Length);
            for (var i = 0; i < minLength; i++)
            {
                if ((_bits[i] & other._bits[i]) != 0)
                {
                    return true;
                }
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return _hash;
        }

        public bool Equals(ComponentMask other)
        {
            return _hash == other._hash;
        }

        public override bool Equals(object? obj)
        {
            return obj is ComponentMask mask && Equals(mask);
        }
    }
}