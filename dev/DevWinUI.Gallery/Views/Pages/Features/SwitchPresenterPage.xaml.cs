namespace DevWinUIGallery.Views;
public sealed partial class SwitchPresenterPage : Page
{
    public SwitchPresenterPage()
    {
        this.InitializeComponent();
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
