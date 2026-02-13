namespace Ais.Games.SnakeGame;

internal class GameWindowSettings
{
    public string Title { get; set; } = string.Empty;

    // Speed compensation is applied dynamically via Y-axis multiplier (Height/Width ratio)
    public int Width { get; set; } = 60;
    public int Height { get; set; } = 20;
}
