using Ais.GameEngine.DependencyInjection.Abstractions;

using Autofac;

namespace Ais.GameEngine.Core.Internal.DI;

internal sealed class AutofacServiceContainer : IServiceContainer
{
    private readonly ILifetimeScope _scope;

    public AutofacServiceContainer(ILifetimeScope scope)
    {
        _scope = scope;
    }

    public T Resolve<T>()
        where T : notnull
    {
        try
        {
            return _scope.Resolve<T>();
        }
        catch (Autofac.Core.DependencyResolutionException ex)
        {
            throw new InvalidOperationException(
                $"Service '{typeof(T).FullName}' is not registered in the DI container.",
                ex);
        }
    }

    public bool TryResolve<T>(out T instance)
        where T : notnull
    {
        try
        {
            var result = _scope.ResolveOptional(typeof(T));
            instance = (T?)result ?? default!;
            return result != null && instance != null;
        }
        catch
        {
            instance = default!;
            return false;
        }
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}
