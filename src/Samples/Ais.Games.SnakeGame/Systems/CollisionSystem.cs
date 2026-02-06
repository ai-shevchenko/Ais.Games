using System.Diagnostics.CodeAnalysis;

using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;

using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class CollisionSystem : EcsSystem
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ISignalPublisher _signalPublisher;
    private readonly GameWindowSettings _windowSettings;

    public CollisionSystem(
        IOptions<GameWindowSettings> windowSettings,
        ICommandExecutor commandExecutor,
        ISignalPublisher signalPublisher)
    {
        _windowSettings = windowSettings.Value;
        _commandExecutor = commandExecutor;
        _signalPublisher = signalPublisher;
    }

    public override void Update(float deltaTime)
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        if (segments.Length == 0)
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
            HandleGameOver(false);
            return;
        }

        if (HasTailCollision(segments, head, headPos))
        {
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
            return;
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
        _ = _signalPublisher.PublishAsync(new GameOverSignal { IsWin = isWin });
    }

    private void HandleFoodEaten()
    {
        _commandExecutor.Execute(new IncreaseScoreCommand { World = World });
        _commandExecutor.Execute(new GrowthSnakeCommand { World = World });
        _commandExecutor.Execute(new SpawnFoodCommand { World = World, WindowSettings = _windowSettings });
    }

    private void HandlePowerUpEaten(IEntity head, IEntity powerUp)
    {
        _commandExecutor.Execute(new ApplyPowerUpCommand { World = World, Head = head, PowerUp = powerUp });
    }
}
