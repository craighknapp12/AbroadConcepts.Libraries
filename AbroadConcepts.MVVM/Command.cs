using System.Windows.Input;

namespace AbroadConcepts.MVVM;

public class Command(Action<Object?> execute, Func<Object?, bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return canExecute == null || canExecute(parameter);

    }

    public void Execute(object? parameter)
    {
        execute(parameter);
    }
}
