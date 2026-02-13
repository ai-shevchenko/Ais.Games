using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Tests.Fixtures;

public sealed class GameLoopEventBusFixture : IDisposable
{
    public void Dispose()
    {
    }

    public TestGameLoopEvent CreateTestEvent(
        string sourceLoopName = "TestSource",
        string? targetLoopName = null)
    {
        return new TestGameLoopEvent { SourceLoopName = sourceLoopName, TargetLoopName = targetLoopName };
    }

    public sealed class TestGameLoopEvent : IGameLoopEvent
    {
        public string SourceLoopName { get; set; } = string.Empty;
        public string? TargetLoopName { get; set; }
    }
}
