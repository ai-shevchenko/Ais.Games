using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class InputSystem : EcsSystem, IInitialize
{
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _inputTask;

    public void Initialize()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _inputTask = Task.Run(ReadInput, _cancellationTokenSource.Token);
    }

    public override void Shutdown()
    {
        _cancellationTokenSource?.Cancel();
        if (_inputTask is not null)
        {
            Task.WhenAny(_inputTask).Wait();
        }
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
                var control = entity.GetComponent<PlayerControlled>(World);
                if (!control.Available)
                {
                    continue;
                }

                ref var velocity = ref entity.GetComponent<Velocity>(World);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        {
                            if (velocity.DirectoinY == 1)
                            {
                                break;
                            }

                            velocity.DirectoinY = -1;
                            velocity.DirectionX = 0;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        {
                            if (velocity.DirectoinY == -1)
                            {
                                break;
                            }

                            velocity.DirectoinY = 1;
                            velocity.DirectionX = 0;
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        {
                            if (velocity.DirectionX == 1)
                            {
                                break;
                            }

                            velocity.DirectionX = -1;
                            velocity.DirectoinY = 0;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        {
                            if (velocity.DirectionX == -1)
                            {
                                break;
                            }

                            velocity.DirectionX = 1;
                            velocity.DirectoinY = 0;
                            break;
                        }
                }
            }
        }
    }
}
