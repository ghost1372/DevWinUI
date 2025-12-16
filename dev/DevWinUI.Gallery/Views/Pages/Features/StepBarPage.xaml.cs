namespace DevWinUIGallery.Views;

public sealed partial class StepBarPage : Page
{
    public ObservableCollection<StepBarDisplayMode> StepBarDisplayModeItems { get; set; } = new ObservableCollection<StepBarDisplayMode>(Enum.GetValues<StepBarDisplayMode>());
    public ObservableCollection<StepBarHeaderDisplayMode> StepBarHeaderDisplayModeItems { get; set; } = new ObservableCollection<StepBarHeaderDisplayMode>(Enum.GetValues<StepBarHeaderDisplayMode>());
    public ObservableCollection<StepStatus> StepStatusItems { get; set; } = new ObservableCollection<StepStatus>(Enum.GetValues<StepStatus>());
    public ObservableCollection<Orientation> OrientationItems { get; set; } = new ObservableCollection<Orientation>(Enum.GetValues<Orientation>());

    public StepBarPage()
    {
        this.InitializeComponent();
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
        StepBarSample.Next();
    }

    private void BtnPrev_Click(object sender, RoutedEventArgs e)
    {
        StepBarSample.Prev();
    }
}
