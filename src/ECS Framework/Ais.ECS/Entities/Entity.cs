using Ais.ECS.Abstractions.Entities;

namespace Ais.ECS.Entities;

public readonly struct Entity : IEntity, IEquatable<Entity>
{
    public Entity(EntityId id, int version)
    {
        Id = id;
        Version = version;
    }

    public EntityId Id { get; }

    public int Version { get; }

    public bool Equals(Entity other)
    {
        return other.Id == Id && other.Version == Version;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Equals(entity);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Version);
    }

    public static bool operator ==(Entity left, Entity right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}
