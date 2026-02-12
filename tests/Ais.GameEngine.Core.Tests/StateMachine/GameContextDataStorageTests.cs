using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.Tests.StateMachine;

public sealed class GameContextDataStorageTests
{
    [Fact(DisplayName = "Проверка установка имени цикла")]
    public void GameContext_SetLoopName_StoresName()
    {
        // Arrange
        var loopName = "TestLoop";

        // Act
        var context = new GameContext { LoopName = loopName };

        // Assert
        Assert.Equal(loopName, context.LoopName);
    }

    [Fact(DisplayName = "Проверка получение текущего состояния")]
    public void GameContext_GetCurrentState_ReturnsState()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };
        var state = new TestGameState();

        // Act
        context.CurrentState = state;

        // Assert
        Assert.Same(state, context.CurrentState);
    }

    [Fact(DisplayName = "Проверка добавление данных в контекст")]
    public void GameContext_TryAdd_StoresValue()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };
        var key = "TestKey";
        var value = "TestValue";

        // Act
        var result = context.TryAdd(key, value);

        // Assert
        Assert.True(result);
        Assert.True(context.TryGet<string>(key, out var retrievedValue));
        Assert.Equal(value, retrievedValue);
    }

    [Fact(DisplayName = "Проверка TryAdd возвращает false при дублирующемся ключе")]
    public void GameContext_TryAddDuplicate_ReturnsFalse()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };
        var key = "TestKey";
        var value1 = "Value1";
        var value2 = "Value2";

        // Act
        var result1 = context.TryAdd(key, value1);
        var result2 = context.TryAdd(key, value2);

        // Assert
        Assert.True(result1);
        Assert.False(result2);
    }

    [Fact(DisplayName = "Проверка Set перезаписывает существующее значение")]
    public void GameContext_SetExisting_OverwritesValue()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };
        var key = "TestKey";
        var value1 = "Value1";
        var value2 = "Value2";

        // Act
        context.Set(key, value1);
        context.Set(key, value2);

        // Assert
        Assert.True(context.TryGet<string>(key, out var retrievedValue));
        Assert.Equal(value2, retrievedValue);
    }

    [Fact(DisplayName = "Проверка TryGet возвращает false для несуществующего ключа")]
    public void GameContext_TryGetNonExistent_ReturnsFalse()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };

        // Act
        var result = context.TryGet<string>("NonExistent", out var value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }

    [Fact(DisplayName = "Проверка Get выбрасывает исключение для несуществующего ключа")]
    public void GameContext_GetNonExistent_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => context.Get<string>("NonExistent"));
        Assert.Contains("NonExistent", exception.Message);
    }

    [Fact(DisplayName = "Проверка хранение различных типов данных")]
    public void GameContext_StoreDifferentTypes_StoresIndependently()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };
        var key = "TestKey";
        var stringValue = "StringValue";
        var intValue = 42;

        // Act
        context.Set(key, stringValue);
        context.Set(key, intValue);

        // Assert
        Assert.True(context.TryGet<string>(key, out var retrievedString));
        Assert.True(context.TryGet<int>(key, out var retrievedInt));
        Assert.Equal(stringValue, retrievedString);
        Assert.Equal(intValue, retrievedInt);
    }

    [Fact(DisplayName = "Проверка получение всех данных контекста")]
    public void GameContext_DataProperty_ReturnsAllEntries()
    {
        // Arrange
        var context = new GameContext { LoopName = "TestLoop" };
        context.Set("Key1", "Value1");
        context.Set("Key2", 42);

        // Act
        var data = context.Data;

        // Assert
        Assert.NotEmpty(data);
        Assert.True(data.Count >= 2);
    }

    private sealed class TestGameState : IGameState
    {
        public Task EnterAsync(GameContext context, CancellationToken stoppingToken = default) => Task.CompletedTask;
        public Task ExecuteAsync(GameContext context, CancellationToken stoppingToken = default) => Task.CompletedTask;
        public Task ExitAsync(GameContext context, CancellationToken stoppingToken = default) => Task.CompletedTask;
    }
}
