namespace Ais.GameEngine.Extensions.Commands.Abstractions;

public interface ICommandQueue : ICommandExecutor
{
    void AddCommand(ICommand command);
    void ExecuteCommands();
    void UndoCommand();
    void RedoCommand();
    void Clear();
}
