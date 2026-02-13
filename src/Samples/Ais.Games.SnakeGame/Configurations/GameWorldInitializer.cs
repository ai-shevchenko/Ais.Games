using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Configurations;

/// <summary>
///     Инициализатор игрового мира (ECS).
///     Содержит логику создания начальной конфигурации сущностей и компонентов.
/// </summary>
internal sealed class GameWorldInitializer
{
    /// <summary>
    ///     Инициализирует игровой мир со змейкой, едой и статистикой.
    /// </summary>
    public static void Initialize(IServiceProvider services, IWorld world)
    {
        var windowSettings = services.GetRequiredService<IOptions<GameWindowSettings>>().Value;
        var commandExecutor = services.GetRequiredService<ICommandExecutor>();

        CreateSnakeHead(world, windowSettings);
        CreateSnakeBody(world, windowSettings);
        CreateScoreEntity(world);
        SpawnInitialFood(commandExecutor, world, windowSettings);
    }

    private static void CreateSnakeHead(IWorld world, GameWindowSettings settings)
    {
        var player = world.CreateEntity();
        player.AddComponent(world, new PlayerControlled { Available = true });
        player.AddComponent(world, new SnakeSegment { IsHead = true, Order = 0 });
        player.AddComponent(world, new Position { X = settings.Width / 2, Y = settings.Height / 2 });
        player.AddComponent(world, new Velocity { DirectionX = 1, DirectionY = 0 });
        player.AddComponent(world, new Sprite { Symbol = '0', Color = ConsoleColor.Green });
        player.AddComponent(world, new GrowthPending { ShouldGrow = false });
    }

    private static void CreateSnakeBody(IWorld world, GameWindowSettings settings)
    {
        const int initialBodyLength = 3;

        for (var i = 0; i < initialBodyLength; i++)
        {
            var segment = world.CreateEntity();
            segment.AddComponent(world, new Position { X = settings.Width / 2 - (i + 1), Y = settings.Height / 2 });
            segment.AddComponent(world, new Sprite { Symbol = 'o', Color = ConsoleColor.DarkGreen });
            segment.AddComponent(world, new SnakeSegment { IsHead = false, Order = i + 1 });
        }
    }

    private static void CreateScoreEntity(IWorld world)
    {
        var scoreEntity = world.CreateEntity();
        scoreEntity.AddComponent(world,
            new Score { Value = 0, FruitsEaten = 0, PowerUpsCollected = 0, ScoreMultiplier = 1 });
    }

    private static void SpawnInitialFood(ICommandExecutor commandExecutor, IWorld world, GameWindowSettings settings)
    {
        commandExecutor.Execute(new SpawnFoodCommand { WindowSettings = settings, World = world });
    }
}
