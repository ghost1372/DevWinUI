using Microsoft.UI.Dispatching;

namespace DevWinUI;

internal sealed partial class SyncDelegateCommand : IDelegateCommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool> _canExecute;
    private readonly DispatcherQueue _dispatcher;

    public event EventHandler? CanExecuteChanged;

    public SyncDelegateCommand(Action<object?> execute, Func<object?, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
        _dispatcher = DispatcherQueue.GetForCurrentThread();
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        if (_dispatcher is not null)
        {
            _dispatcher.TryEnqueue(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }
        else
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
