using System.Windows.Input;

namespace DevWinUIGallery.Views;

public sealed partial class CommandPage : Page
{
    private ICommand SimpleCommand { get; }
    private ICommand CommandWithCanExecute { get; }
    private ICommand CommandWithParameter { get; }
    public CommandPage()
    {
        this.InitializeComponent();
        SimpleCommand = DelegateCommand.Create(OnSimpleCommand);
        CommandWithCanExecute = DelegateCommand.Create(OnSimpleCommand, CanExecuteCommandWithCanExecute);
        CommandWithParameter = DelegateCommand.Create(OnCommandWithParameter, CanExecuteCommandWithParameter);
    }

    private bool CanExecuteCommandWithParameter(object? parameter)
    {
        return TGCommandWithParameter.IsOn;
    }

    private void OnCommandWithParameter(object? parameter)
    {
        MessageBox.ShowInformation(App.MainWindow, $"Command Executed with Parameter: {parameter}");
    }

    private bool CanExecuteCommandWithCanExecute()
    {
        return TGCommandWithCanExecute.IsOn;
    }

    private void OnSimpleCommand()
    {
        MessageBox.ShowInformation(App.MainWindow, "Command Executed");
    }
}
