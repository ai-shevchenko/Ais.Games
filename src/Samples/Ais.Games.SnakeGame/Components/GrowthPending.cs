using Ais.ECS.Abstractions.Components;

namespace Ais.Games.SnakeGame.Components;

internal struct GrowthPending : IComponent
{
    public int PendingCount;
    public bool ShouldGrow;
}
