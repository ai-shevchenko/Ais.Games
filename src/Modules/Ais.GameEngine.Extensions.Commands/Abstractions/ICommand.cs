namespace Ais.GameEngine.Extensions.Commands.Abstractions;

public interface ICommand
{
    void Execute();
    void Undo();
}
