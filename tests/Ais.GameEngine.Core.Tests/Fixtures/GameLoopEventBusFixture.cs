using Ais.GameEngine.Core.Abstractions;
using NSubstitute;

namespace Ais.GameEngine.Core.Tests.Fixtures;

public sealed class GameLoopEventBusFixture : IDisposable
{
    public GameLoopEventBusFixture()
    {
    }

    public TestGameLoopEvent CreateTestEvent(
        string sourceLoopName = "TestSource",
        string? targetLoopName = null)
    {
        return new TestGameLoopEvent
        {
            SourceLoopName = sourceLoopName,
            TargetLoopName = targetLoopName
        };
    }

    public void Dispose()
    {
    }

    public sealed class TestGameLoopEvent : IGameLoopEvent
    {
        public string SourceLoopName { get; set; } = string.Empty;
        public string? TargetLoopName { get; set; }
    }
}
