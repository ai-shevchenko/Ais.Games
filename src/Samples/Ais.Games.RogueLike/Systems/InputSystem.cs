using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class InputSystem : EcsSystem, IInitialize
{
    private CancellationTokenSource _cancellationTokenSource;
    private Task _inputTask;

    public void Initialize()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _inputTask = Task.Run(ReadInput, _cancellationTokenSource.Token);
    }

    public override void Shutdown()
    {
        _cancellationTokenSource?.Cancel();
    }

    private void ReadInput()
    {
        var result = World.CreateQuery()
            .With<PlayerControlled>()
            .With<Velocity>()
            .GetResult();

        while (true)
        {
            var key = Console.ReadKey(true).Key;

            foreach (var entity in result.Entities)
            {
                var velocity = entity.GetComponent<Velocity>(World);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    {
                        velocity.DirectoinY = -1;
                        velocity.DirectionX = 0;
                        break;
                    }
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    {
                        velocity.DirectoinY = 1;
                        velocity.DirectionX = 0;
                        break;
                    }
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    {
                        velocity.DirectionX = -1;
                        velocity.DirectoinY = 0;
                        break;
                    }
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    {
                        velocity.DirectionX = 1;
                        velocity.DirectoinY = 0;
                        break;
                    }
                }
            }
        }
    }
}
