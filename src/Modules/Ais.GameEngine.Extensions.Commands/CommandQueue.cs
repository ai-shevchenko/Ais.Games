using Ais.GameEngine.Extensions.Commands.Abstractions;

namespace Ais.GameEngine.Extensions.Commands;

public sealed class CommandQueue : ICommandQueue, ICommandExecutor
{
    private readonly Stack<ICommand> _commands = [];
    private readonly Stack<ICommand> _redo = [];
    private readonly Stack<ICommand> _undo = [];

    public void AddCommand(ICommand command)
    {
        _commands.Push(command);
    }

    public void ExecuteCommands()
    {
        while (_commands.TryPop(out var command))
        {
            Execute(command);
        }
    }

    public void Execute(ICommand command)
    {
        if (_redo.Count > 0)
        {
            _redo.Clear();
        }

        _undo.Push(command);
        command.Execute();
    }

    public void RedoCommand()
    {
        if (_redo.Count == 0)
        {
            return;
        }

        var redo = _redo.Pop();
        _undo.Push(redo);

        redo.Execute();
    }

    public void UndoCommand()
    {
        if (_undo.Count == 0)
        {
            return;
        }

        var undo = _undo.Pop();
        _redo.Push(undo);

        undo.Undo();
    }

    public void Clear()
    {
        _redo.Clear();
        _undo.Clear();
    }
}
