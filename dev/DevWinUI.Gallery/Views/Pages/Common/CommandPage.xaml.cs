namespace DevWinUIGallery.Views;

public sealed partial class CommandPage : Page
{
    private IDelegateCommand SimpleCommand { get; }
    private IDelegateCommand CommandWithCanExecute { get; }
    private IDelegateCommand CommandWithParameter { get; }
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

    private async void OnCommandWithParameter(object? parameter)
    {
        await MessageBox.ShowAsync($"Command Executed with Parameter: {parameter}");
    }

    private bool CanExecuteCommandWithCanExecute()
    {
        return TGCommandWithCanExecute.IsOn;
    }

    private async void OnSimpleCommand()
    {
        await MessageBox.ShowAsync("Command Executed");
    }

    private void TGCommandWithParameter_Toggled(object sender, RoutedEventArgs e)
    {
        if (SimpleCommand != null && CommandWithCanExecute != null && CommandWithParameter != null)
        {
            SimpleCommand.RaiseCanExecuteChanged();
            CommandWithCanExecute.RaiseCanExecuteChanged();
            CommandWithParameter.RaiseCanExecuteChanged();
        }
    }
}
