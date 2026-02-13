using System.Diagnostics.CodeAnalysis;

using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Extensions;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Events;

using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class CollisionSystem : EcsSystem
{
    private readonly IGameContextAccessor _accessor;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IGameLoopEventBus _eventBus;
    private readonly GameWindowSettings _windowSettings;

    public CollisionSystem(
        IOptions<GameWindowSettings> windowSettings,
        ICommandExecutor commandExecutor,
        IGameLoopEventBus eventBus,
        IGameContextAccessor accessor)
    {
        _windowSettings = windowSettings.Value;
        _commandExecutor = commandExecutor;
        _eventBus = eventBus;
        _accessor = accessor;
    }

    public override void Update(float deltaTime)
    {
        CheckSnakeAnomalies();
        CheckFoodAnomalies();

        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        if (segments.Length == 0)
        {
            return;
        }

        var headEntity = FindSnakeHead(segments);
        if (headEntity == null)
        {
            return;
        }

        var playerControl = headEntity.GetComponent<PlayerControlled>(World);
        if (!playerControl.Available)
        {
            return;
        }

        IEntity? head = null;
        var orderedSegments = new List<(IEntity Entity, SnakeSegment Segment)>(segments.Length);
        foreach (var e in segments)
        {
            var seg = e.GetComponent<SnakeSegment>(World);
            orderedSegments.Add((e, seg));
        }

        if (orderedSegments.Count == 0)
        {
            return;
        }

        orderedSegments.Sort((a, b) => a.Segment.Order.CompareTo(b.Segment.Order));
        foreach (var pair in orderedSegments)
        {
            if (pair.Segment.IsHead)
            {
                head = pair.Entity;
                break;
            }
        }

        if (head is null)
        {
            return;
        }

        var headPos = head.GetComponent<Position>(World);

        if (HasWallCollision(headPos))
        {
            PublishWallCollision(headPos);
            HandleGameOver(false);
            return;
        }

        if (HasTailCollision(segments, head, headPos))
        {
            PublishTailCollision(headPos);
            HandleGameOver(false);
            return;
        }

        if (HasFoodCollision(headPos))
        {
            HandleFoodEaten();
            return;
        }

        if (HasPowerUpCollision(head, headPos, out var powerUp))
        {
            HandlePowerUpEaten(head, powerUp);
        }
    }

    private bool HasPowerUpCollision(IEntity head, Position headPos, [NotNullWhen(true)] out IEntity? powerUp)
    {
        var powerUps = World.CreateQuery()
            .With<PowerUp>()
            .With<Position>()
            .GetResult()
            .Entities;

        foreach (var powerUpEntity in powerUps)
        {
            var pos = powerUpEntity.GetComponent<Position>(World);
            if (!Equals(headPos, pos))
            {
                continue;
            }

            powerUp = powerUpEntity;
            return true;
        }

        powerUp = default;
        return false;
    }

    private bool HasFoodCollision(Position headPos)
    {
        var foodEntities = World.CreateQuery()
            .With<Food>()
            .With<Position>()
            .GetResult()
            .Entities;

        foreach (var food in foodEntities)
        {
            var foodPos = food.GetComponent<Position>(World);
            if (Equals(foodPos, headPos))
            {
                World.DestroyEntity(food);
                return true;
            }
        }

        return false;
    }

    private bool HasTailCollision(ReadOnlySpan<IEntity> segments, IEntity head, Position headPos)
    {
        foreach (var segment in segments)
        {
            if (Equals(segment, head))
            {
                continue;
            }

            var pos = segment.GetComponent<Position>(World);
            if (Equals(pos, headPos))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasWallCollision(Position headPos)
    {
        return headPos.X <= 0
               || headPos.X >= _windowSettings.Width + 1
               || headPos.Y <= 0
               || headPos.Y >= _windowSettings.Height + 1;
    }

    private void HandleGameOver(bool isWin)
    {
        _commandExecutor.Execute(new StopSnakeCommand { World = World });
        _ = _eventBus.PublishAsync(new GameOverEvent
        {
            SourceLoopName = _accessor.CurrentContext!.LoopName, IsWin = isWin
        });
    }

    private void HandleFoodEaten()
    {
        _commandExecutor.Execute(new IncreaseScoreCommand { World = World });
        _commandExecutor.Execute(new GrowthSnakeCommand { World = World });
        _commandExecutor.Execute(new SpawnFoodCommand { World = World, WindowSettings = _windowSettings });

        PublishFoodEaten();
        PublishSnakeGrowth();
    }

    private void HandlePowerUpEaten(IEntity head, IEntity powerUp)
    {
        _commandExecutor.Execute(new ApplyPowerUpCommand { World = World, Head = head, PowerUp = powerUp });

        var powerUpComp = powerUp.GetComponent<PowerUp>(World);
        PublishPowerUpCollected(powerUpComp.Type);
    }

    private void PublishWallCollision(Position position)
    {
        _ = _eventBus.PublishAsync(new WallCollisionEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
            PositionX = position.X,
            PositionY = position.Y
        });
    }

    private void PublishTailCollision(Position position)
    {
        _ = _eventBus.PublishAsync(new TailCollisionEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
            PositionX = position.X,
            PositionY = position.Y
        });
    }

    private void PublishFoodEaten()
    {
        var scoreEntities = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        if (scoreEntities.Length > 0)
        {
            var score = scoreEntities[0].GetComponent<Score>(World);
            var scoreGained = 10 * (score.ScoreMultiplier <= 0 ? 1 : score.ScoreMultiplier);
            _ = _eventBus.PublishAsync(new FoodEatenEvent
            {
                SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                ScoreGained = scoreGained,
                TotalScore = score.Value
            });
        }
    }

    private void PublishSnakeGrowth()
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .GetResult()
            .Entities;

        _ = _eventBus.PublishAsync(new SnakeGrowthEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown", NewLength = segments.Length
        });
    }

    private void PublishPowerUpCollected(PowerUpType powerUpType)
    {
        _ = _eventBus.PublishAsync(new PowerUpCollectedEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
            PowerUpType = powerUpType,
            EffectDuration = 5f
        });
    }

    private void CheckSnakeAnomalies()
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .GetResult()
            .Entities;

        var headCount = 0;
        var maxOrder = -1;
        var segmentList = new List<(IEntity Entity, SnakeSegment Segment)>(segments.Length);

        foreach (var segment in segments)
        {
            if (!World.GetStore<SnakeSegment>().Contains(segment))
            {
                continue;
            }

            var seg = segment.GetComponent<SnakeSegment>(World);
            if (seg.IsHead)
            {
                headCount++;
            }

            if (seg.Order > maxOrder)
            {
                maxOrder = seg.Order;
            }

            segmentList.Add((segment, seg));
        }

        if (headCount > 1)
        {
            _ = _eventBus.PublishAsync(new MultipleHeadsDetectedEvent
            {
                SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown", HeadCount = headCount
            });
        }

        if (maxOrder + 1 != segments.Length)
        {
            _ = _eventBus.PublishAsync(new BrokenSnakeChainEvent
            {
                SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                SegmentCount = segments.Length,
                MaxOrder = maxOrder
            });

            RepairSnakeOrder(segmentList);
        }

        var positionSet = new HashSet<(int, int)>();
        foreach (var segment in segments)
        {
            if (!World.GetStore<SnakeSegment>().Contains(segment) ||
                !World.GetStore<Position>().Contains(segment))
            {
                continue;
            }

            var pos = segment.GetComponent<Position>(World);
            if (!positionSet.Add((pos.X, pos.Y)))
            {
                _ = _eventBus.PublishAsync(new EntityClippingEvent
                {
                    SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                    PositionX = pos.X,
                    PositionY = pos.Y,
                    Entity1Type = "SnakeSegment",
                    Entity2Type = "SnakeSegment"
                });
            }
        }
    }

    private void RepairSnakeOrder(List<(IEntity Entity, SnakeSegment Segment)> segmentList)
    {
        // Сортируем по текущему Order (это может помочь, но не гарантирует правильную последовательность)
        segmentList.Sort((a, b) => a.Segment.Order.CompareTo(b.Segment.Order));

        for (var i = 0; i < segmentList.Count; i++)
        {
            var entity = segmentList[i].Entity;
            ref var seg = ref entity.GetComponent<SnakeSegment>(World);
            if (seg.Order != i)
            {
                seg.Order = i;
            }
        }

        // Публикуем событие об исправлении
        _ = _eventBus.PublishAsync(new SnakeChainRepairedEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown", NewLength = segmentList.Count
        });
    }

    private void CheckFoodAnomalies()
    {
        var foodEntities = World.CreateQuery()
            .With<Food>()
            .With<Position>()
            .GetResult()
            .Entities;

        var foodPositions = new Dictionary<(int, int), int>();
        foreach (var food in foodEntities)
        {
            var pos = food.GetComponent<Position>(World);
            var key = (pos.X, pos.Y);
            if (!foodPositions.TryAdd(key, 1))
            {
                foodPositions[key]++;
            }
        }

        foreach (var kvp in foodPositions)
        {
            if (kvp.Value > 1)
            {
                _ = _eventBus.PublishAsync(new DuplicateFoodEvent
                {
                    SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                    PositionX = kvp.Key.Item1,
                    PositionY = kvp.Key.Item2,
                    FoodCount = kvp.Value
                });
            }
        }
    }

    private IEntity? FindSnakeHead(ReadOnlySpan<IEntity> segments)
    {
        foreach (var entity in segments)
        {
            if (!World.GetStore<SnakeSegment>().Contains(entity))
            {
                continue;
            }

            var segment = entity.GetComponent<SnakeSegment>(World);
            if (segment.IsHead)
            {
                return entity;
            }
        }

        return null;
    }
}
