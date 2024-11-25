using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public class MyDataModel
{
    public string? Name { get; set; }

    public string? Info { get; set; }

    public string? LinkDescription { get; set; }

    public string? Url { get; set; }
}
public sealed partial class SettingsControls : Page
{
    public ObservableCollection<MyDataModel> MyDataSet = new() {
        new()
        {
            Name = "First Item",
            Info = "More about first item.",
            LinkDescription = "Click here for more on first item.",
            Url = "https://microsoft.com/",
        },
        new()
        {
            Name = "Second Item",
            Info = "More about second item.",
            LinkDescription = "Click here for more on second item.",
            Url = "https://xbox.com/",
        },
        new()
        {
            Name = "Third Item",
            Info = "More about third item.",
            LinkDescription = "Click here for more on third item.",
            Url = "https://toolkitlabs.dev/",
        },
    };
    public bool IsCardEnabled { get; set; } = true;
    public bool IsCardExpanded { get; set; } = false;
    public SettingsControls()
    {
        this.InitializeComponent();
    }
    private async void OnCardClicked(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.microsoft.com"));
    }
}
