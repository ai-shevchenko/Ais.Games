namespace Ais.GameEngine.Modules.Abstractions;

/// <summary>
///     Обогатитель модулей
/// </summary>
public interface IModuleEnricher
{
    /// <summary>
    ///     Обогатить систему модулями
    /// </summary>
    void Enrich();
}
