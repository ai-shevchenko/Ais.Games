using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Extensions.Commands;

internal sealed class CommandExecutor : BaseHook, ILateUpdate
{
    private readonly ICommandQueue _commandQueue;

    public CommandExecutor(ICommandQueue commandQueue)
    {
        _commandQueue = commandQueue;
    }

    public void LateUpdate(float deltaTime)
    {
        _commandQueue.ExecuteCommands();
    }
}
