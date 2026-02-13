using Ais.GameEngine.Core.Abstractions;
using Ais.Games.SnakeGame.Abstractions;
using Ais.Games.SnakeGame.Configurations;

namespace Ais.Games.SnakeGame;

/// <summary>
///     Главный класс управления игрой "Змейка".
///     Инкапсулирует логику инициализации и запуска всех игровых циклов.
/// </summary>
internal sealed class SnakeGame : IGameSessionManager
{
    private readonly IGameLoopConfigurator _configurator;
    private readonly IGameEngine _gameEngine;
    private readonly GameSession _gameSession;
    private readonly CancellationTokenSource _stoppingTokenSource;
    private IGameLoop? _gameOverLoop;

    private IGameLoop? _loggingLoop;
    private IGameLoop? _mainLoop;
    private IGameLoop? _menuLoop;

    public SnakeGame(IGameEngine gameEngine, GameSession gameSession, CancellationTokenSource stoppingTokenSource)
        : this(gameEngine, gameSession, stoppingTokenSource, new DefaultGameLoopConfigurator())
    {
    }

    internal SnakeGame(IGameEngine gameEngine, GameSession gameSession, CancellationTokenSource stoppingTokenSource,
        IGameLoopConfigurator configurator)
    {
        _gameEngine = gameEngine;
        _gameSession = gameSession;
        _stoppingTokenSource = stoppingTokenSource;
        _configurator = configurator;
    }

    public GameState CurrentState => _gameSession.State;

    /// <summary>
    ///     Инициализирует все игровые циклы.
    /// </summary>
    public void InitializeGameLoops()
    {
        _loggingLoop = _gameEngine.CreateGameLoop("logging", CreateLoggingLoopSettings);
        _menuLoop = _gameEngine.CreateGameLoop("menu", CreateMenuLoopSettings);
        _gameOverLoop = _gameEngine.CreateGameLoop("gameover", CreateGameOverLoopSettings);
        _mainLoop = _gameEngine.CreateGameLoop("main", CreateMainLoopSettings);
    }

    /// <summary>
    ///     Запускает игру с главного меню.
    /// </summary>
    public async Task RunAsync()
    {
        if (_loggingLoop == null || _menuLoop == null || _gameOverLoop == null || _mainLoop == null)
        {
            throw new InvalidOperationException("Game loops not initialized. Call InitializeGameLoops() first.");
        }

        await _loggingLoop.StartAsync(_stoppingTokenSource.Token);
        await _menuLoop.StartAsync(_stoppingTokenSource.Token);

        await WaitForMenuSelectionAsync();

        if (_gameSession.State == GameState.Start)
        {
            await RunGameSessionAsync();
        }

        await _gameEngine.StopAsync();
    }

    private async Task WaitForMenuSelectionAsync()
    {
        while (_gameSession.State == GameState.None && !_stoppingTokenSource.IsCancellationRequested)
        {
            await Task.Delay(50);
        }
    }

    private async Task RunGameSessionAsync()
    {
        while (_gameSession.State == GameState.Start && !_stoppingTokenSource.IsCancellationRequested)
        {
            await RunSingleGameAsync();

            if (_gameSession.State is not (GameState.Start or GameState.Exit))
            {
                break;
            }

            _gameSession.SetResult(GameState.None);
        }
    }

    private async Task RunSingleGameAsync()
    {
        await _menuLoop!.PauseAsync(_stoppingTokenSource.Token);

        await _mainLoop!.StartAsync(_stoppingTokenSource.Token);

        while (_gameSession.State == GameState.Start && !_stoppingTokenSource.IsCancellationRequested)
        {
            await Task.Delay(50);
        }

        await _mainLoop.PauseAsync(_stoppingTokenSource.Token);

        if (_gameSession.State is GameState.Won or GameState.Lost)
        {
            await ShowGameOverAsync();
        }
    }

    private async Task ShowGameOverAsync()
    {
        _gameSession.SetResult(GameState.None);
        await _gameOverLoop!.StartAsync(_stoppingTokenSource.Token);

        while (_gameSession.State == GameState.None && !_stoppingTokenSource.IsCancellationRequested)
        {
            await Task.Delay(50);
        }

        await _gameOverLoop.PauseAsync(_stoppingTokenSource.Token);

        if (_gameSession.State == GameState.Start)
        {
            _gameSession.SetResult(GameState.None);
            await _menuLoop!.ResumeAsync(_stoppingTokenSource.Token);

            while (_gameSession.State == GameState.None && !_stoppingTokenSource.IsCancellationRequested)
            {
                await Task.Delay(50);
            }

            if (_gameSession.State == GameState.Start)
            {
                await _menuLoop.PauseAsync(_stoppingTokenSource.Token);
            }
        }
    }

    private void CreateLoggingLoopSettings(GameLoopBuilderSettings settings)
    {
        _configurator.ConfigureLoggingLoop(settings);
    }

    private void CreateMenuLoopSettings(GameLoopBuilderSettings settings)
    {
        _configurator.ConfigureMenuLoop(settings);
    }

    private void CreateGameOverLoopSettings(GameLoopBuilderSettings settings)
    {
        _configurator.ConfigureGameOverLoop(settings);
    }

    private void CreateMainLoopSettings(GameLoopBuilderSettings settings)
    {
        _configurator.ConfigureMainGameLoop(settings);
    }
}
