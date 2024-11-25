using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;
public sealed partial class SwitchPresenterPage : Page
{
    public ObservableCollection<Animal> Items
    {
        get { return (ObservableCollection<Animal>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<Animal>), typeof(SwitchPresenterPage), new PropertyMetadata(new ObservableCollection<Animal>(Enum.GetValues<Animal>())));

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
