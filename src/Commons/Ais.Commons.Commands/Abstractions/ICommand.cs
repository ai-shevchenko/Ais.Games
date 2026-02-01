namespace Ais.Commons.Commands.Abstractions;

public interface ICommand
{
    void Execute();
    void Undo();
}
