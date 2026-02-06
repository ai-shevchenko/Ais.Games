using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;

using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class PowerUpSpawnSystem : EcsSystem
{
    private readonly GameWindowSettings _settings;
    private readonly float _spawnIntervalSeconds = 10f;
    private readonly ICommandExecutor _commandExecutor;

    private float _timeSinceLastSpawn;

    public PowerUpSpawnSystem(IOptions<GameWindowSettings> settings, ICommandExecutor commandExecutor)
    {
        _settings = settings.Value;
        _commandExecutor = commandExecutor;
    }

    public override void Update(float deltaTime)
    {
        _timeSinceLastSpawn += deltaTime;
        if (_timeSinceLastSpawn < _spawnIntervalSeconds)
        {
            return;
        }

        _timeSinceLastSpawn = 0f;

        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        _commandExecutor.Execute(new SpawnPowerUpCommand { World = World, WindowSettings = _settings });
    }
}
