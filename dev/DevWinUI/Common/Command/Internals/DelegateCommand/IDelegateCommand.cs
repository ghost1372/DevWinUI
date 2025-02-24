using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace DevWinUI;

public interface IDelegateCommand : ICommand
{
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "This method raise an existing event")]
    void RaiseCanExecuteChanged();
}
