﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Dispatching;

namespace DevWinUI;

internal sealed partial class AsyncDelegateCommand : IDelegateCommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool> _canExecute;
    private readonly DispatcherQueue _dispatcher;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;

    public AsyncDelegateCommand(Func<object?, Task> execute, Func<object?, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
        _dispatcher = DispatcherQueue.GetForCurrentThread();
    }

    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && _canExecute.Invoke(parameter);
    }

    [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Must be void")]
    public async void Execute(object? parameter)
    {
        if (_isExecuting)
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            await _execute.Invoke(parameter);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        var canExecuteChanged = CanExecuteChanged;
        if (canExecuteChanged is not null)
        {
            if (_dispatcher is not null)
            {
                _dispatcher.TryEnqueue(() => canExecuteChanged.Invoke(this, EventArgs.Empty));
            }
            else
            {
                canExecuteChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
