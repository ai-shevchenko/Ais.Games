using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Signals;

internal readonly record struct SnakeMoveSignal(
    Position LastPosition,
    Position CurrentPosition)
    : ISignal;
