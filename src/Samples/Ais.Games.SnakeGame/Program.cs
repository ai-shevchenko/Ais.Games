using Ais.GameEngine.Core;
using Ais.Games.SnakeGame;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

var builder = GameEngineBuilder.Create(args);

var gameSession = new GameSession();
builder.ConfigureGameServices((context, services) =>
{
    var settings = context.Configuration.GetRequiredSection(nameof(GameWindowSettings));
    services.Configure<GameWindowSettings>(settings);
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

using (var gameEngine = builder.Build())
{
    var snakeGame = new SnakeGame(gameEngine, gameSession, stoppingTokenSource);
    snakeGame.InitializeGameLoops();
    await snakeGame.RunAsync();
}
