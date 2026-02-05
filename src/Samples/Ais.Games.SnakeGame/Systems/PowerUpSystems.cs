using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Components;

using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class PowerUpSpawnSystem : EcsSystem
{
    private readonly Random _random = Random.Shared;
    private readonly GameWindowSettings _settings;
    private readonly float _spawnIntervalSeconds = 10f;

    private float _timeSinceLastSpawn;

    public PowerUpSpawnSystem(IOptions<GameWindowSettings> settings)
    {
        _settings = settings.Value;
    }

    public override void Update(float deltaTime)
    {
        _timeSinceLastSpawn += deltaTime;
        if (_timeSinceLastSpawn < _spawnIntervalSeconds)
        {
            return;
        }

        _timeSinceLastSpawn = 0f;

        SpawnPowerUp(World, _settings);
    }

    private void SpawnPowerUp(IWorld world, GameWindowSettings window)
    {
        var entities = world.CreateQuery()
            .With<Position>()
            .GetResult()
            .Entities;

        var (newX, newY) = (0, 0);

        while (true)
        {
            newX = _random.Next(1, window.Width - 1);
            newY = _random.Next(1, window.Height - 1);

            var occupied = false;
            foreach (var entity in entities)
            {
                var pos = entity.GetComponent<Position>(world);
                if (pos.X == newX && pos.Y == newY)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
            {
                break;
            }
        }

        var type = _random.Next(0, 2) == 0 ? PowerUpType.SpeedBoost : PowerUpType.DoubleScore;
        var powerUp = world.CreateEntity();
        powerUp.AddComponent(world, new Position { X = newX, Y = newY });
        powerUp.AddComponent(world, new PowerUp { Type = type, TimeToLiveSeconds = 8f, ElapsedSeconds = 0f });

        var sprite = type switch
        {
            PowerUpType.SpeedBoost => new Sprite { Symbol = 'S', Color = ConsoleColor.Cyan },
            PowerUpType.DoubleScore => new Sprite { Symbol = '$', Color = ConsoleColor.Yellow },
            _ => new Sprite { Symbol = '?', Color = ConsoleColor.White }
        };

        powerUp.AddComponent(world, sprite);
    }
}

/// <summary>
///     Обновляет время жизни паверапов и удаляет истекшие.
/// </summary>
internal sealed class PowerUpLifetimeSystem : EcsSystem
{
    public override void Update(float deltaTime)
    {
        var result = World.CreateQuery()
            .With<PowerUp>()
            .GetResult()
            .Entities;

        foreach (var entity in result)
        {
            ref var powerUp = ref entity.GetComponent<PowerUp>(World);
            powerUp.ElapsedSeconds += deltaTime;
            if (powerUp.ElapsedSeconds >= powerUp.TimeToLiveSeconds)
            {
                World.DestroyEntity(entity);
            }
        }
    }
}

/// <summary>
///     Обновляет активные эффекты паверапов (таймеры, множитель очков).
/// </summary>
internal sealed class PowerUpEffectSystem : EcsSystem
{
    public override void Update(float deltaTime)
    {
        // Ищем голову змейки
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
        foreach (var e in segments)
        {
            var seg = e.GetComponent<SnakeSegment>(World);
            if (seg.IsHead)
            {
                head = e;
                break;
            }
        }

        if (head is null)
        {
            return;
        }

        // Обновляем активный эффект на голове
        if (World.GetStore<ActivePowerUpEffect>().Contains(head))
        {
            ref var effect = ref head.GetComponent<ActivePowerUpEffect>(World);
            effect.RemainingSeconds -= deltaTime;

            if (effect.RemainingSeconds <= 0f)
            {
                // Эффект закончился — сбрасываем множитель очков
                ResetScoreMultiplier();
                World.GetStore<ActivePowerUpEffect>().Remove(head);
                return;
            }

            // Если эффект удвоения очков — поддерживаем множитель
            if (effect.Type == PowerUpType.DoubleScore)
            {
                EnsureScoreMultiplier(2);
            }
        }
        else
        {
            // Нет активного эффекта — множитель по умолчанию
            ResetScoreMultiplier();
        }
    }

    private void EnsureScoreMultiplier(int multiplier)
    {
        var scores = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        if (scores.Length == 0)
        {
            return;
        }

        foreach (var entity in scores)
        {
            ref var score = ref entity.GetComponent<Score>(World);
            score.ScoreMultiplier = multiplier;
        }
    }

    private void ResetScoreMultiplier()
    {
        var scores = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        foreach (var entity in scores)
        {
            ref var score = ref entity.GetComponent<Score>(World);
            score.ScoreMultiplier = 1;
        }
    }
}
