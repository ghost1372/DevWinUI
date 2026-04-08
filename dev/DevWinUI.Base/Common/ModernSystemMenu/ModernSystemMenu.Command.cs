using System.Windows.Input;

namespace DevWinUI;
public partial class ModernSystemMenu
{
    public ICommand RestoreCommand { get; }
    public ICommand MoveCommand { get; }
    public ICommand SizeCommand { get; }
    public ICommand MinimizeCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand CloseCommand { get; }

    private bool CanExecuteRestore()
    {
        return IsWindowMaximized;
    }

    private void Restore()
    {
        overlappedPresenter.Restore();
    }

    private bool CanExecuteMove(object? parameter)
    {
        return IsWindowMaximized == false;
    }

    private void Move(object? parameter)
    {
        var menuItem = parameter as MenuFlyoutItem;
        OnMoveClicked(menuItem);
    }

    private bool CanExecuteSize(object? parameter)
    {
        return IsWindowMaximized == false;
    }

    private void Size(object? parameter)
    {
        var menuItem = parameter as MenuFlyoutItem;
        OnSizeClicked(menuItem);
    }

    private bool CanExecuteMinimize()
    {
        return overlappedPresenter.IsMinimizable;
    }

    private void Minimize()
    {
        overlappedPresenter.Minimize();
    }

    private bool CanExecuteMaximize()
    {
        return IsWindowMaximized == false && overlappedPresenter.IsMaximizable;
    }

    private void Maximize()
    {
        overlappedPresenter.Maximize();
    }

    private bool CanExecuteClose()
    {
        return true;
    }

    private void Close()
    {
        Application.Current.Exit();
    }
}
