namespace Ais.GameEngine.Extensions.Commands.Abstractions;

public interface ICommandExecutor
{
    void Execute(ICommand command);
}
