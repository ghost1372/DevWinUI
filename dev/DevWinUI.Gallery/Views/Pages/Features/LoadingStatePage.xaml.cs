namespace DevWinUIGallery.Views;

public sealed partial class LoadingStatePage : Page
{
    public ObservableCollection<LoadingStateMode> Modes { get; set; } = new ObservableCollection<LoadingStateMode>(Enum.GetValues<LoadingStateMode>());

    public LoadingStatePage()
    {
        InitializeComponent();
    }

    private void ControlExample_ResetRequested(object sender, EventArgs e)
    {
        MyLoadingState.Restart();
    }
}
