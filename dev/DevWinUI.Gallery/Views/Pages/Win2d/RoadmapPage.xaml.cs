namespace DevWinUIGallery.Views;

public sealed partial class RoadmapPage : Page
{
    public ObservableCollection<RoadmapMode> ModeItems { get; set; } = new ObservableCollection<RoadmapMode>(Enum.GetValues<RoadmapMode>());
    public ObservableCollection<RoadmapPlacement> PlacementItems { get; set; } = new ObservableCollection<RoadmapPlacement>(Enum.GetValues<RoadmapPlacement>());
    public ObservableCollection<RoadmapItemStatus> StatusItems { get; set; } = new ObservableCollection<RoadmapItemStatus>(Enum.GetValues<RoadmapItemStatus>());
    public ObservableCollection<RoadmapToggle> ToggleItems { get; set; } = new ObservableCollection<RoadmapToggle>(Enum.GetValues<RoadmapToggle>());

    public RoadmapPage()
    {
        InitializeComponent();
    }

    private void OnReplayBootClick(object sender, RoutedEventArgs e)
    {
        NeonRoadmap.PlayBootAnimation();
    }
}
