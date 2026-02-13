using Ais.GameEngine.Core.Abstractions;

namespace Ais.Games.SnakeGame.Abstractions;

/// <summary>
///     Интерфейс для управления игровым сеансом.
///     Позволяет отделить логику управления состояниями от конкретной реализации.
/// </summary>
internal interface IGameSessionManager
{
    /// <summary>
    ///     Получает текущее состояние игровой сессии.
    /// </summary>
    GameState CurrentState { get; }

    /// <summary>
    ///     Инициализирует все игровые циклы.
    /// </summary>
    void InitializeGameLoops();

    /// <summary>
    ///     Запускает игру асинхронно.
    /// </summary>
    Task RunAsync();
}

/// <summary>
///     Интерфейс для конфигурации игровых циклов.
///     Позволяет кастомизировать инициализацию циклов.
/// </summary>
internal interface IGameLoopConfigurator
{
    /// <summary>
    ///     Конфигурирует цикл логирования.
    /// </summary>
    void ConfigureLoggingLoop(GameLoopBuilderSettings settings);

    /// <summary>
    ///     Конфигурирует цикл главного меню.
    /// </summary>
    void ConfigureMenuLoop(GameLoopBuilderSettings settings);

    /// <summary>
    ///     Конфигурирует цикл экрана "Game Over".
    /// </summary>
    void ConfigureGameOverLoop(GameLoopBuilderSettings settings);

    /// <summary>
    ///     Конфигурирует основной игровой цикл.
    /// </summary>
    void ConfigureMainGameLoop(GameLoopBuilderSettings settings);
}
