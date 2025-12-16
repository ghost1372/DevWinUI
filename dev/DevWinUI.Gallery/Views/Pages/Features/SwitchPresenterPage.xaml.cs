namespace DevWinUIGallery.Views;
public sealed partial class SwitchPresenterPage : Page
{
    public ObservableCollection<Animal> SwitchPresenterItems { get; set; } = new ObservableCollection<Animal>(Enum.GetValues<Animal>());

    public BaseViewModel ViewModel { get; }
    public SwitchPresenterPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        this.InitializeComponent();
    }

    private void CodeValidator_TextChanging(Microsoft.UI.Xaml.Controls.TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (sender.Parent is StackPanel stackPanel)
        {
            var textBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault();
            if (textBlock != null)
            {
                bool isValid = TextBoxExtensions.GetIsValid(sender);
                textBlock.Visibility = isValid ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}

public enum Animal
{
    Bunny,
    Cat,
    Dog,
    Giraffe,
    Llama,
    Otter,
    Owl,
    Parrot,
    Squirrel
}
public enum CheckStatus
{
    Error,
    Warning,
    Success,
}
public partial class TemplateInformation
{
    public string? Header { get; set; }

    public string? Regex { get; set; }

    public string? PlaceholderText { get; set; }
}
