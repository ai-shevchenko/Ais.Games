using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Tests.Events;

public sealed class GameLoopEventArgsTests
{
    [Fact(DisplayName = "Проверка установка имени игрового цикла")]
    public void GameLoopEventArgs_SetLoopName_StoresName()
    {
        // Arrange
        var args = new GameLoopEventArgs();
        var loopName = "TestLoop";

        // Act
        args.LoopName = loopName;

        // Assert
        Assert.Equal(loopName, args.LoopName);
    }

    [Fact(DisplayName = "Проверка установка временной метки события")]
    public void GameLoopEventArgs_SetTimestamp_StoresTimestamp()
    {
        // Arrange
        var args = new GameLoopEventArgs();
        var timestamp = DateTime.UtcNow;

        // Act
        args.Timestamp = timestamp;

        // Assert
        Assert.Equal(timestamp, args.Timestamp);
    }

    [Fact(DisplayName = "Проверка по умолчанию временная метка примерно равна текущему времени")]
    public void GameLoopEventArgs_DefaultTimestamp_IsCurrentUtcTime()
    {
        // Arrange
        var timeBefore = DateTime.UtcNow;

        // Act
        var args = new GameLoopEventArgs();

        // Assert
        var timeAfter = DateTime.UtcNow;
        Assert.InRange(args.Timestamp, timeBefore.AddSeconds(-1), timeAfter.AddSeconds(1));
    }

    [Fact(DisplayName = "Проверка по умолчанию имя цикла пусто")]
    public void GameLoopEventArgs_DefaultLoopName_IsEmpty()
    {
        // Arrange & Act
        var args = new GameLoopEventArgs();

        // Assert
        Assert.Equal(string.Empty, args.LoopName);
    }
}

public sealed class GameLoopErrorEventArgsTests
{
    [Fact(DisplayName = "Проверка установка сообщения об ошибке")]
    public void GameLoopErrorEventArgs_SetErrorMessage_StoresMessage()
    {
        // Arrange
        var args = new GameLoopErrorEventArgs();
        var errorMessage = "Test error message";

        // Act
        args.ErrorMessage = errorMessage;

        // Assert
        Assert.Equal(errorMessage, args.ErrorMessage);
    }

    [Fact(DisplayName = "Проверка установка исключения")]
    public void GameLoopErrorEventArgs_SetException_StoresException()
    {
        // Arrange
        var args = new GameLoopErrorEventArgs();
        var exception = new InvalidOperationException("Test exception");

        // Act
        args.Exception = exception;

        // Assert
        Assert.Same(exception, args.Exception);
    }

    [Fact(DisplayName = "Проверка наследование от GameLoopEventArgs")]
    public void GameLoopErrorEventArgs_InheritsFromGameLoopEventArgs()
    {
        // Arrange & Act
        var args = new GameLoopErrorEventArgs();

        // Assert
        Assert.IsAssignableFrom<GameLoopEventArgs>(args);
    }

    [Fact(DisplayName = "Проверка установка имени цикла через базовый класс")]
    public void GameLoopErrorEventArgs_SetLoopName_InheritsFromBase()
    {
        // Arrange
        var args = new GameLoopErrorEventArgs();
        var loopName = "ErrorLoop";

        // Act
        args.LoopName = loopName;

        // Assert
        Assert.Equal(loopName, args.LoopName);
    }

    [Fact(DisplayName = "Проверка установка временной метки через базовый класс")]
    public void GameLoopErrorEventArgs_SetTimestamp_InheritsFromBase()
    {
        // Arrange
        var args = new GameLoopErrorEventArgs();
        var timestamp = DateTime.UtcNow;

        // Act
        args.Timestamp = timestamp;

        // Assert
        Assert.Equal(timestamp, args.Timestamp);
    }

    [Fact(DisplayName = "Проверка по умолчанию сообщение об ошибке пусто")]
    public void GameLoopErrorEventArgs_DefaultErrorMessage_IsEmpty()
    {
        // Arrange & Act
        var args = new GameLoopErrorEventArgs();

        // Assert
        Assert.Equal(string.Empty, args.ErrorMessage);
    }

    [Fact(DisplayName = "Проверка по умолчанию исключение null")]
    public void GameLoopErrorEventArgs_DefaultException_IsNull()
    {
        // Arrange & Act
        var args = new GameLoopErrorEventArgs();

        // Assert
        Assert.Null(args.Exception);
    }
}
