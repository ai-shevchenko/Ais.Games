using Ais.Commons.Commands.Abstractions;
using Ais.ECS.Extensions;
using Ais.GameEngine.Core;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Systems;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Serilog;

var builder = GameEngineBuilder.Create(
    new GameEngineBuilderSettings
    {
        Args = args,
        DllModules = 
        {
            { "main", [ "./Ais.GameEngine.Extensions.dll" ] }
        }
    });

builder.ConfigureGameServices((context, services) =>
{
    var settings = context.Configuration.GetRequiredSection(nameof(GameWindowSettings));
    services.Configure<GameWindowSettings>(settings);
    
    services.AddEcs()
        .WithSystem<InputSystem>()
        .WithSystem<MovementSystem>()
        .WithSystem<RenderSystem>();
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
using var mainLoop = gameEngine.GetOrCreateGameLoop("main", settings =>
{
    settings.InitializeEcsLoop((services, world) =>
    {
        var windowSettings = services.GetRequiredService<IOptions<GameWindowSettings>>().Value;

        var player = world.CreateEntity();
        player.AddComponent<PlayerControlled>(world);
        player.AddComponent(world, new SnakeSegment
        {
            IsHead = true,
        });
        player.AddComponent(world, new Position
        {
            X = windowSettings.Width / 2,
            Y = windowSettings.Height / 2
        });
        player.AddComponent(world, new Velocity
        {
            DirectionX = 1,
            DirectoinY = 0
        });
        player.AddComponent(world, new Sprite
        {
            Symbol = '0',
            Color = ConsoleColor.Green
        });
        
        for (int i = 0; i < 3; i++)
        {
            var segment = world.CreateEntity();
            segment.AddComponent(world, new Position
            {
                X = windowSettings.Width / 2 - (i + 1),
                Y = windowSettings.Height / 2
            });
            segment.AddComponent(world, new Sprite
            {
                Symbol = 'o',
                Color = ConsoleColor.DarkGreen
            });
            segment.AddComponent<SnakeSegment>(world);
        }

        services.GetRequiredService<ICommandExecutor>()
            .Execute(new SpawnFoodCommand
            {
                WindowSettings = windowSettings,
                World = world,
            });
    });
});

gameEngine.Start(stoppingTokenSource.Token);

while (!stoppingTokenSource.IsCancellationRequested) { }

gameEngine.Stop();
