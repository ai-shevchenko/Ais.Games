using Ais.ECS;
using Ais.ECS.Extensions;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Systems;

namespace Ais.Games.SnakeGame.Tests;

public class PowerUpSystemsTests
{
    [Fact]
    public void PowerUpLifetimeSystem_RemovesExpiredPowerUp()
    {
        var world = new World(new EcsSettings());
        var lifetimeSystem = new PowerUpLifetimeSystem();
        world.AddSystem(lifetimeSystem);

        var entity = world.CreateEntity();
        entity.AddComponent(world, new PowerUp
        {
            Type = PowerUpType.SpeedBoost,
            TimeToLiveSeconds = 1f,
            ElapsedSeconds = 0.9f
        });

        // Первый апдейт – ещё жив
        world.UpdateSystems(0.05f);
        Assert.Equal(1, world.GetAllEntities().Length);

        // Второй апдейт – должен быть удалён
        world.UpdateSystems(0.1f);
        Assert.Equal(0, world.GetAllEntities().Length);
    }

    [Fact]
    public void PowerUpEffectSystem_ChangesScoreMultiplier()
    {
        var world = new World(new EcsSettings());
        var effectSystem = new PowerUpEffectSystem();
        world.AddSystem(effectSystem);

        var head = world.CreateEntity();
        head.AddComponent(world, new SnakeSegment { IsHead = true, Order = 0 });
        head.AddComponent(world, new Position { X = 0, Y = 0 });
        head.AddComponent(world, new ActivePowerUpEffect
        {
            Type = PowerUpType.DoubleScore,
            RemainingSeconds = 5f
        });

        var stats = world.CreateEntity();
        stats.AddComponent(world, new Score
        {
            Value = 0,
            FruitsEaten = 0,
            PowerUpsCollected = 0,
            ScoreMultiplier = 1
        });

        world.UpdateSystems(0.1f);

        var score = stats.GetComponent<Score>(world);
        Assert.Equal(2, score.ScoreMultiplier);
    }
}

