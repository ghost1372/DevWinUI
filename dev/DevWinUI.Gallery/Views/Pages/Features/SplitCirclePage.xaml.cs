namespace DevWinUIGallery.Views;

public sealed partial class SplitCirclePage : Page
{
    public ObservableCollection<Orientation> OrientationItems { get; set; } = new ObservableCollection<Orientation>(Enum.GetValues<Orientation>());

    public SplitCirclePage()
    {
        InitializeComponent();
    }
}
