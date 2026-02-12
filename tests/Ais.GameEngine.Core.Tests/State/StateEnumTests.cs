using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Tests.State;

public sealed class GameLoopStateTests
{
    [Fact(DisplayName = "Проверка значение Stopped равно 0")]
    public void GameLoopState_Stopped_HasValueZero()
    {
        // Act
        var value = (int)GameLoopState.Stopped;

        // Assert
        Assert.Equal(0, value);
    }

    [Fact(DisplayName = "Проверка значение Initializing равно 1")]
    public void GameLoopState_Initializing_HasValueOne()
    {
        // Act
        var value = (int)GameLoopState.Initializing;

        // Assert
        Assert.Equal(1, value);
    }

    [Fact(DisplayName = "Проверка значение Running равно 2")]
    public void GameLoopState_Running_HasValueTwo()
    {
        // Act
        var value = (int)GameLoopState.Running;

        // Assert
        Assert.Equal(2, value);
    }

    [Fact(DisplayName = "Проверка значение Paused равно 3")]
    public void GameLoopState_Paused_HasValueThree()
    {
        // Act
        var value = (int)GameLoopState.Paused;

        // Assert
        Assert.Equal(3, value);
    }

    [Fact(DisplayName = "Проверка значение Stopping равно 4")]
    public void GameLoopState_Stopping_HasValueFour()
    {
        // Act
        var value = (int)GameLoopState.Stopping;

        // Assert
        Assert.Equal(4, value);
    }

    [Fact(DisplayName = "Проверка значение Failed равно 5")]
    public void GameLoopState_Failed_HasValueFive()
    {
        // Act
        var value = (int)GameLoopState.Failed;

        // Assert
        Assert.Equal(5, value);
    }
}

public sealed class EngineStateTests
{
    [Fact(DisplayName = "Проверка значение NotInitialized равно 0")]
    public void EngineState_NotInitialized_HasValueZero()
    {
        // Act
        var value = (int)EngineState.NotInitialized;

        // Assert
        Assert.Equal(0, value);
    }

    [Fact(DisplayName = "Проверка значение Idle равно 1")]
    public void EngineState_Idle_HasValueOne()
    {
        // Act
        var value = (int)EngineState.Idle;

        // Assert
        Assert.Equal(1, value);
    }

    [Fact(DisplayName = "Проверка значение Running равно 2")]
    public void EngineState_Running_HasValueTwo()
    {
        // Act
        var value = (int)EngineState.Running;

        // Assert
        Assert.Equal(2, value);
    }

    [Fact(DisplayName = "Проверка значение Stopping равно 3")]
    public void EngineState_Stopping_HasValueThree()
    {
        // Act
        var value = (int)EngineState.Stopping;

        // Assert
        Assert.Equal(3, value);
    }

    [Fact(DisplayName = "Проверка значение Stopped равно 4")]
    public void EngineState_Stopped_HasValueFour()
    {
        // Act
        var value = (int)EngineState.Stopped;

        // Assert
        Assert.Equal(4, value);
    }

    [Fact(DisplayName = "Проверка значение Failed равно 5")]
    public void EngineState_Failed_HasValueFive()
    {
        // Act
        var value = (int)EngineState.Failed;

        // Assert
        Assert.Equal(5, value);
    }
}
