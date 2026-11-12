namespace DevWinUIGallery.Views;

public sealed partial class ParticleGlobePage : Page
{
    public ObservableCollection<ParticleGlobeTheme> ThemeItems { get; } = new(Enum.GetValues<ParticleGlobeTheme>());
    public ObservableCollection<ParticleGlobeQuality> QualityItems { get; } = new(Enum.GetValues<ParticleGlobeQuality>());

    public ParticleGlobePage()
    {
        InitializeComponent();
    }

    private void OnGlobeCountryClicked(object sender, ParticleGlobeCountryEventArgs e)
    {
        GlobeStatusText.Text = $"Selected: {e.Name} ({e.Code})";
    }

    private void OnAddSampleEventClick(object sender, RoutedEventArgs e)
    {
        Globe.AddEvent(37.7749, -122.4194, 8, label: "Sample event");
        GlobeStatusText.Text = "Added a sample event near San Francisco.";
    }
}
