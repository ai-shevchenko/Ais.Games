namespace Ais.ECS.Abstractions.Systems;

/// <summary>
/// Система
/// </summary>
public interface ISystem
{
    /// <summary>
    /// Инициализировать систему
    /// </summary>
    /// <param name="context">Контекст</param>
    void Initialize(SystemContext context);

    /// <summary>
    /// Обновить систему
    /// </summary>
    /// <param name="deltaTime">Разница времени между кадрами</param>
    void Update(float deltaTime);

    /// <summary>
    /// Выключить систему
    /// </summary>
    void Shutdown();
}