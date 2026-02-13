using Ais.ECS.Extensions;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Events;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class InputSystem : EcsSystem, IInitialize
{
    private readonly IGameContextAccessor _accessor;
    private readonly IGameLoopEventBus _eventBus;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _inputTask;

    public InputSystem(IGameLoopEventBus eventBus, IGameContextAccessor accessor)
    {
        _eventBus = eventBus;
        _accessor = accessor;
    }

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
                // Check if components still exist
                if (!World.GetStore<PlayerControlled>().Contains(entity) ||
                    !World.GetStore<Velocity>().Contains(entity))
                {
                    continue;
                }

                var control = entity.GetComponent<PlayerControlled>(World);
                if (!control.Available)
                {
                    continue;
                }

                ref var velocity = ref entity.GetComponent<Velocity>(World);
                var oldDirectionX = velocity.DirectionX;
                var oldDirectionY = velocity.DirectionY;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    {
                        if (velocity.DirectionY == 1)
                        {
                            PublishInvalidMovement(0, -1, oldDirectionX, oldDirectionY);
                            break;
                        }

                        velocity.DirectionY = -1;
                        velocity.DirectionX = 0;
                        PublishDirectionChanged(0, -1);
                        break;
                    }
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    {
                        if (velocity.DirectionY == -1)
                        {
                            PublishInvalidMovement(0, 1, oldDirectionX, oldDirectionY);
                            break;
                        }

                        velocity.DirectionY = 1;
                        velocity.DirectionX = 0;
                        PublishDirectionChanged(0, 1);
                        break;
                    }
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    {
                        if (velocity.DirectionX == 1)
                        {
                            PublishInvalidMovement(-1, 0, oldDirectionX, oldDirectionY);
                            break;
                        }

                        velocity.DirectionX = -1;
                        velocity.DirectionY = 0;
                        PublishDirectionChanged(-1, 0);
                        break;
                    }
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    {
                        if (velocity.DirectionX == -1)
                        {
                            PublishInvalidMovement(1, 0, oldDirectionX, oldDirectionY);
                            break;
                        }

                        velocity.DirectionX = 1;
                        velocity.DirectionY = 0;
                        PublishDirectionChanged(1, 0);
                        break;
                    }
                }
            }
        }
    }

    private void PublishDirectionChanged(int directionX, int directionY)
    {
        _ = _eventBus.PublishAsync(new DirectionChangedEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
            DirectionX = directionX,
            DirectionY = directionY
        });
    }

    private void PublishInvalidMovement(int attemptedX, int attemptedY, int currentX, int currentY)
    {
        _ = _eventBus.PublishAsync(new SnakeInvalidMovementEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
            AttemptedDirectionX = attemptedX,
            AttemptedDirectionY = attemptedY,
            CurrentDirectionX = currentX,
            CurrentDirectionY = currentY
        });
    }
}
