namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Менеджер игровых циклов — управляет созданием и поиском игровых циклов.
/// Предоставляет доступ к циклам по типизированным именам для избежания "magic strings".
/// </summary>
public interface IGameLoopManager
{
    /// <summary>
    /// Список всех активных игровых циклов.
    /// </summary>
    IReadOnlyList<IGameLoop> GameLoops { get; }

    /// <summary>
    /// Получить игровой цикл по имени.
    /// </summary>
    /// <param name="name">Типизированное имя игрового цикла (например, GameLoopNames.Main).</param>
    /// <returns>Игровой цикл.</returns>
    /// <exception cref="KeyNotFoundException">Если цикл с таким именем не найден.</exception>
    IGameLoop GetGameLoop(string name);

    /// <summary>
    /// Попытаться получить игровой цикл по имени.
    /// </summary>
    /// <param name="name">Типизированное имя игрового цикла.</param>
    /// <param name="gameLoop">Найденный игровой цикл, если успешно.</param>
    /// <returns>true, если цикл найден; иначе false.</returns>
    bool TryGetGameLoop(string name, out IGameLoop gameLoop);

    /// <summary>
    /// Получить или создать новый игровой цикл асинхронно.
    /// </summary>
    /// <param name="name">Типизированное имя игрового цикла.</param>
    /// <param name="configure">Дополнительная конфигурация цикла.</param>
    /// <returns>Игровой цикл.</returns>
    IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings>? configure = null);

    /// <summary>
    /// Проверить наличие игрового цикла.
    /// </summary>
    /// <param name="name">Типизированное имя игрового цикла.</param>
    /// <returns>true, если цикл существует; иначе false.</returns>
    bool HasGameLoop(string name);

    /// <summary>
    /// Удалить игровой цикл по имени асинхронно.
    /// Цикл будет корректно остановлен перед удалением.
    /// </summary>
    /// <param name="name">Типизированное имя игрового цикла.</param>
    /// <param name="timeout">Максимальное время ожидания остановки цикла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>true, если цикл успешно удалён; иначе false.</returns>
    Task<bool> RemoveGameLoopAsync(
        string name,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}
