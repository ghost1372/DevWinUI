namespace DevWinUIGallery.Views;

public sealed partial class StarBorderPage : Page
{
    public ObservableCollection<StarDirection> DirectionItems { get; set; } = new ObservableCollection<StarDirection>(Enum.GetValues<StarDirection>());
    public ObservableCollection<StarHoverMode> HoverModeItems { get; set; } = new ObservableCollection<StarHoverMode>(Enum.GetValues<StarHoverMode>());
    public ObservableCollection<StarBorderTheme> ThemeItems { get; set; } = new ObservableCollection<StarBorderTheme>(Enum.GetValues<StarBorderTheme>());

    public StarBorderPage()
    {
        InitializeComponent();
    }
}
