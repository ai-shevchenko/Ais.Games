using Ais.ECS.Extensions;
using Ais.GameEngine.Core;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Systems;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Serilog;

var builder = GameEngineBuilder.Create(args);

var gameSession = new GameSession();
builder.ConfigureGameServices((context, services) =>
{
    var settings = context.Configuration.GetRequiredSection(nameof(GameWindowSettings));
    services.Configure<GameWindowSettings>(settings);

    // Глобальное состояние сессии игры
    services.AddSingleton(gameSession);
});

builder.ConfigureGameLogging((context, logging) =>
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(context.Configuration)
        .CreateLogger();

    logging.AddSerilog(dispose: true);
});

var stoppingTokenSource = new CancellationTokenSource();

using var gameEngine = builder.Build();

using var menuLoop = gameEngine.CreateGameLoop("menu", settings =>
{
    settings.GameServices
        .AddEcs()
        .WithSystem<MenuRenderSystem>()
        .WithSystem<MenuInputSystem>();
});

using var mainLoop = gameEngine.CreateGameLoop("main", settings =>
{
    settings.GameServices
        .AddEcs()
        .WithSystem<InputSystem>()
        .WithSystem<MovementSystem>()
        .WithSystem<CollisionSystem>()
        .WithSystem<PowerUpSpawnSystem>()
        .WithSystem<PowerUpLifetimeSystem>()
        .WithSystem<PowerUpEffectSystem>()
        .WithSystem<GameOverSignalHandler>()
        .WithSystem<RenderSystem>()
        .WithWorldSetup((services, world) =>
        {
            var windowSettings = services.GetRequiredService<IOptions<GameWindowSettings>>().Value;

            var player = world.CreateEntity();
            player.AddComponent<PlayerControlled>(world);
            player.AddComponent(world, new SnakeSegment { IsHead = true, Order = 0 });
            player.AddComponent(world, new Position { X = windowSettings.Width / 2, Y = windowSettings.Height / 2 });
            player.AddComponent(world, new Velocity { DirectionX = 1, DirectoinY = 0 });
            player.AddComponent(world, new Sprite { Symbol = '0', Color = ConsoleColor.Green });

            for (var i = 0; i < 3; i++)
            {
                var segment = world.CreateEntity();
                segment.AddComponent(world,
                    new Position { X = windowSettings.Width / 2 - (i + 1), Y = windowSettings.Height / 2 });
                segment.AddComponent(world, new Sprite { Symbol = 'o', Color = ConsoleColor.DarkGreen });
                segment.AddComponent(world, new SnakeSegment { IsHead = false, Order = i + 1 });
            }

            // Инициализируем счёт
            var scoreEntity = world.CreateEntity();
            scoreEntity.AddComponent(world, new Score
            {
                Value = 0,
                FruitsEaten = 0,
                PowerUpsCollected = 0,
                ScoreMultiplier = 1
            });

            services.GetRequiredService<ICommandExecutor>()
                .Execute(new SpawnFoodCommand { WindowSettings = windowSettings, World = world });
        });
});

gameSession.SetResult(GameResult.None);
menuLoop.Start(stoppingTokenSource.Token);

while (gameSession.Result == GameResult.None && !stoppingTokenSource.IsCancellationRequested)
{
    await Task.Delay(50);
}

gameEngine.Stop();

