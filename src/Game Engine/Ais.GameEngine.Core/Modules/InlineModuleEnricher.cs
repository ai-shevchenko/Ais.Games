using Ais.GameEngine.Modules.Abstractions;

namespace Ais.GameEngine.Core.Modules;

internal sealed class InlineModuleEnricher : IModuleEnricher
{
    private readonly Action<IKeyedModuleLoader> _enricher;

    public InlineModuleEnricher(Action<IKeyedModuleLoader> enricher)
    {
        _enricher = enricher;
    }

    public void Enrich(IKeyedModuleLoader moduleLoader)
    {
        _enricher(moduleLoader);
    }
}
