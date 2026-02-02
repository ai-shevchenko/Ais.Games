namespace Ais.Commons.Commands.Abstractions;

public interface ICommandExecutor
{
    void Execute(ICommand command);
}
