using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class GameOverRenderSystem : EcsSystem, IRender
{
    private readonly GameSession _session;

    public GameOverRenderSystem(GameSession session)
    {
        _session = session;
    }

    public void Render(float alpha)
    {
        Console.Clear();
        Console.CursorVisible = false;

        var message = _session.State switch
        {
            GameState.Won => "YOU WON!",
            GameState.Lost => "GAME OVER",
            GameState.Exit => "GOOD BYE",
            _ => "GAME OVER"
        };

        Console.SetCursorPosition(5, 5);
        Console.WriteLine(message);

        Console.SetCursorPosition(5, 7);
        Console.WriteLine("Press ENTER to go back to menu or ESC to exit.");
    }
}