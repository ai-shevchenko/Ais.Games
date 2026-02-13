using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Modules.Abstractions.Extensions;
using Ais.Games.SnakeGame.Abstractions;
using Ais.Games.SnakeGame.Hooks;
using Ais.Games.SnakeGame.Systems;

namespace Ais.Games.SnakeGame.Configurations;

/// <summary>
///     Стандартная конфигурация игровых циклов для Змейки.
/// </summary>
internal sealed class DefaultGameLoopConfigurator : IGameLoopConfigurator
{
    public void ConfigureLoggingLoop(GameLoopBuilderSettings settings)
    {
        settings.GameServices.AddSingletonHook<LogSignalsHook>();
    }

    public void ConfigureMenuLoop(GameLoopBuilderSettings settings)
    {
        settings.GameServices.AddSingletonHook<MainMenuHook>();
    }

    public void ConfigureGameOverLoop(GameLoopBuilderSettings settings)
    {
        settings.GameServices.AddSingletonHook<GameOverMenuHook>();
    }

    public void ConfigureMainGameLoop(GameLoopBuilderSettings settings)
    {
        settings.GameServices
            .AddEcs()
            .WithSystem<InputSystem>()
            .WithSystem<MovementSystem>()
            .WithSystem<CollisionSystem>()
            .WithSystem<PowerUpSpawnSystem>()
            .WithSystem<PowerUpLifetimeSystem>()
            .WithSystem<PowerUpEffectSystem>()
            .WithSystem<AnomalyDetectionSystem>()
            .WithSystem<GameOverSignalHandler>()
            .WithSystem<RenderSystem>()
            .WithWorldSetup(GameWorldInitializer.Initialize);
    }
}
