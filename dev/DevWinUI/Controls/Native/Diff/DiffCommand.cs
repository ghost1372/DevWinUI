using System.Windows.Input;

namespace DevWinUI;

internal sealed partial class DiffCommand(Action<object?> execute) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => parameter is DiffRow { IsExpander: true };

    public void Execute(object? parameter) => execute(parameter);
}
