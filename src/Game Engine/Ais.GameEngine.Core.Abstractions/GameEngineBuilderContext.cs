using Microsoft.Extensions.Configuration;

namespace Ais.GameEngine.Core.Abstractions;

/// <summary>
/// Контекст конфигурации игрового движка
/// </summary>
/// <param name="Configuration">Конфигурация</param>
public record GameEngineBuilderContext(IConfiguration Configuration);
