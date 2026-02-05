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

        if (headPos.X <= 0 || headPos.X >= _windowSettings.Width + 1 ||
            headPos.Y <= 0 || headPos.Y >= _windowSettings.Height + 1)
        {
            HandleGameOver(false);
            return;
        }

        foreach (var segment in segments)
        {
            if (segment == head)
            {
                continue;
            }

            var pos = segment.GetComponent<Position>(World);
            if (pos.X == headPos.X && pos.Y == headPos.Y)
            {
                HandleGameOver(false);
                return;
            }
        }

        var foodEntities = World.CreateQuery()
            .With<Food>()
            .With<Position>()
            .GetResult()
            .Entities;

        foreach (var food in foodEntities)
        {
            var foodPos = food.GetComponent<Position>(World);
            if (foodPos.X == headPos.X && foodPos.Y == headPos.Y)
            {
                OnFoodEaten();
                World.DestroyEntity(food);
                return;
            }
        }

        var powerUps = World.CreateQuery()
            .With<PowerUp>()
            .With<Position>()
            .GetResult()
            .Entities;

        foreach (var powerUpEntity in powerUps)
        {
            var pos = powerUpEntity.GetComponent<Position>(World);
            if (pos.X != headPos.X || pos.Y != headPos.Y)
            {
                continue;
            }

            ref var powerUp = ref powerUpEntity.GetComponent<PowerUp>(World);

            var scoreEntities = World.CreateQuery()
                .With<Score>()
                .GetResult()
                .Entities;

            if (scoreEntities.Length > 0)
            {
                ref var score = ref scoreEntities[0].GetComponent<Score>(World);
                score.PowerUpsCollected += 1;
            }

            var effectStore = World.GetStore<ActivePowerUpEffect>();
            if (!effectStore.Contains(head))
            {
                head.AddComponent(World, new ActivePowerUpEffect { Type = powerUp.Type, RemainingSeconds = 5f });
            }
            else
            {
                ref var effect = ref head.GetComponent<ActivePowerUpEffect>(World);
                effect.Type = powerUp.Type;
                effect.RemainingSeconds = 5f;
            }

            World.DestroyEntity(powerUpEntity);
            return;
        }
    }

    private void HandleGameOver(bool isWin)
    {
        var heads = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Velocity>()
            .GetResult()
            .Entities;

        foreach (var entity in heads)
        {
            var velocity = entity.GetComponent<Velocity>(World);
            velocity.DirectionX = 0;
            velocity.DirectoinY = 0;

            ref var v = ref entity.GetComponent<Velocity>(World);
            v = velocity;
        }

        _ = _signalPublisher.PublishAsync(new GameOverSignal { IsWin = isWin });
    }

    private void OnFoodEaten()
    {
        var scoreEntities = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        IEntity scoreEntity;
        Score score;

        if (scoreEntities.Length == 0)
        {
            scoreEntity = World.CreateEntity();
            score = new Score { Value = 0, FruitsEaten = 0, PowerUpsCollected = 0, ScoreMultiplier = 1 };
            scoreEntity.AddComponent(World, score);
        }
        else
        {
            scoreEntity = scoreEntities[0];
            score = scoreEntity.GetComponent<Score>(World);
        }

        var points = 10 * (score.ScoreMultiplier <= 0 ? 1 : score.ScoreMultiplier);
        score.Value += points;
        score.FruitsEaten += 1;

        ref var scoreRef = ref scoreEntity.GetComponent<Score>(World);
        scoreRef = score;

        var segmentsSpan = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        if (segmentsSpan.Length == 0)
        {
            return;
        }

        var segmentsList = new List<(IEntity Entity, SnakeSegment Segment, Position Position)>(segmentsSpan.Length);
        foreach (var e in segmentsSpan)
        {
            var seg = e.GetComponent<SnakeSegment>(World);
            var pos = e.GetComponent<Position>(World);
            segmentsList.Add((e, seg, pos));
        }

        segmentsList.Sort((a, b) => a.Segment.Order.CompareTo(b.Segment.Order));

        var tail = segmentsList[^1];
        var newSegment = World.CreateEntity();
        newSegment.AddComponent(World, new SnakeSegment { IsHead = false, Order = tail.Segment.Order + 1 });
        newSegment.AddComponent(World, tail.Position);
        newSegment.AddComponent(World, new Sprite { Symbol = 'o', Color = ConsoleColor.DarkGreen });

        _commandExecutor.Execute(new SpawnFoodCommand { World = World, WindowSettings = _windowSettings });
    }
}
