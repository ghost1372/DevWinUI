namespace DevWinUIGallery.Views;

public sealed partial class LoadingIndicatorPage : Page
{
    public ObservableCollection<LoadingIndicatorMode> ModesItems { get; set; } = new ObservableCollection<LoadingIndicatorMode>(Enum.GetValues<LoadingIndicatorMode>());

    public LoadingIndicatorPage()
    {
        InitializeComponent();
    }
}
