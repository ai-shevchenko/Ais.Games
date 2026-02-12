using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Tests.Events;

public sealed class GameLoopEventTests
{
    [Fact(DisplayName = "Проверка установка имени исходящего цикла")]
    public void GameLoopEvent_SetSourceLoopName_StoresName()
    {
        // Arrange
        var evt = new TestGameLoopEvent();
        var loopName = "SourceLoop";

        // Act
        evt.SourceLoopName = loopName;

        // Assert
        Assert.Equal(loopName, evt.SourceLoopName);
    }

    [Fact(DisplayName = "Проверка установка имени целевого цикла")]
    public void GameLoopEvent_SetTargetLoopName_StoresName()
    {
        // Arrange
        var evt = new TestGameLoopEvent();
        var loopName = "TargetLoop";

        // Act
        evt.TargetLoopName = loopName;

        // Assert
        Assert.Equal(loopName, evt.TargetLoopName);
    }

    [Fact(DisplayName = "Проверка целевой цикл может быть null для broadcast")]
    public void GameLoopEvent_TargetLoopNameIsNull_AllowsBroadcast()
    {
        // Arrange
        var evt = new TestGameLoopEvent();

        // Act
        evt.TargetLoopName = null;

        // Assert
        Assert.Null(evt.TargetLoopName);
    }

    private sealed class TestGameLoopEvent : IGameLoopEvent
    {
        public string SourceLoopName { get; set; } = string.Empty;
        public string? TargetLoopName { get; set; }
    }
}
