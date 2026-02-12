using Ais.GameEngine.Core.Internal.ModulesSystem;
using Ais.GameEngine.Modules.Abstractions;

namespace Ais.GameEngine.Core.Internal.Helpers;

internal sealed class ModuleEnricherManager
{
    private readonly List<IModuleEnricher> _enrichers = [];

    public void AddEnricher(IModuleEnricher enricher)
    {
        _enrichers.Add(enricher ?? throw new ArgumentNullException(nameof(enricher)));
    }

    public void AddEnricher(Action<IModuleLoader> enricher)
    {
        _enrichers.Add(new InlineModuleEnricher(enricher));
    }

    public void EnrichAll(IModuleLoader moduleLoader)
    {
        foreach (var enricher in _enrichers)
        {
            enricher.Enrich(moduleLoader);
        }
    }
}
