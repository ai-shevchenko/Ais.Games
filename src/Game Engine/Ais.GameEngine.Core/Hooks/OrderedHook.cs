using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core.Hooks;

public sealed class OrderedHook<THook> : BaseHook
    where THook : IHook
{
    public required THook Hook { get; init; }
    public required int Order { get; init; }
}
