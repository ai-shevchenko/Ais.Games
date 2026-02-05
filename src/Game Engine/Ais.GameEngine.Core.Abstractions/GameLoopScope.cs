namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
///     Область существования игрового цикла
/// </summary>
public sealed class GameLoopScope : IDisposable
{
    /// <summary>
    ///     Конструктор
    /// </summary>
    /// <param name="name">Наименование игрового цикла</param>
    /// <param name="gameLoop">Экземпляр игрового цикла</param>
    /// <param name="scope">Область игрового цикла</param>
    public GameLoopScope(string name, IGameLoop gameLoop, IDisposable scope)
    {
        Name = name;
        GameLoop = gameLoop;
        Scope = scope;
    }

    /// <summary>
    ///     Наименование игрового цикла
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Экземпляр игрового цикла
    /// </summary>
    public IGameLoop GameLoop { get; }

    /// <summary>
    ///     Область игрового цикла
    /// </summary>
    public IDisposable Scope { get; }

    public void Dispose()
    {
        GameLoop.Dispose();
        Scope.Dispose();
    }
}
