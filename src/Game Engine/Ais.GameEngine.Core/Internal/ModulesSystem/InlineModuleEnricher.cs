using Ais.GameEngine.Modules.Abstractions;

namespace Ais.GameEngine.Core.Internal.ModulesSystem;

internal sealed class InlineModuleEnricher : IModuleEnricher
{
    private readonly Action<IModuleLoader> _enricher;

    public InlineModuleEnricher(Action<IModuleLoader> enricher)
    {
        _enricher = enricher;
    }

    public void Enrich(IModuleLoader moduleLoader)
    {
        _enricher(moduleLoader);
    }
}
