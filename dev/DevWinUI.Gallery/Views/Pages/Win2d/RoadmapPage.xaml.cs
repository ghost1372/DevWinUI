namespace DevWinUIGallery.Views;

public sealed partial class RoadmapPage : Page
{
    public RoadmapPage()
    {
        InitializeComponent();
    }

    private void OnReplayBootClick(object sender, RoutedEventArgs e)
    {
        NeonRoadmap.PlayBootAnimation();
    }
}
