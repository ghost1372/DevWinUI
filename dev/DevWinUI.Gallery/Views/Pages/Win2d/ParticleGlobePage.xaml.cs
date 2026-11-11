namespace DevWinUIGallery.Views;

public sealed partial class ParticleGlobePage : Page
{
    public ObservableCollection<ParticleGlobeTheme> ThemeItems { get; } = new(Enum.GetValues<ParticleGlobeTheme>());
    public ObservableCollection<ParticleGlobeQuality> QualityItems { get; } = new(Enum.GetValues<ParticleGlobeQuality>());
    public ObservableCollection<ParticleGlobeFlatMapMode> FlatMapModeItems { get; } = new(Enum.GetValues<ParticleGlobeFlatMapMode>());
    public ObservableCollection<ParticleGlobeCountryOption> CountryItems { get; } = new()
    {
        new("United States", "USA"),
        new("Canada", "CAN"),
        new("United Kingdom", "GBR"),
        new("France", "FRA"),
        new("Germany", "DEU"),
        new("Japan", "JPN"),
        new("Australia", "AUS"),
        new("Brazil", "BRA"),
        new("India", "IND"),
        new("Iran", "IRN")
    };

    public ParticleGlobePage()
    {
        InitializeComponent();
    }

    private void OnGlobeCountryClicked(object sender, ParticleGlobeCountryEventArgs e)
    {
        CmbCountry.SelectedItem = CountryItems.FirstOrDefault(country => country.Code == e.Code);
        GlobeStatusText.Text = $"Selected: {e.Name} ({e.Code})";
    }

    private void OnCountrySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbCountry.SelectedItem is ParticleGlobeCountryOption country)
        {
            Globe.SelectedCountryCode = country.Code;
            GlobeStatusText.Text = $"Selected: {country.Name} ({country.Code})";
        }
    }

    private void OnAddSampleEventClick(object sender, RoutedEventArgs e)
    {
        Globe.AddEvent(37.7749, -122.4194, 8, label: "Sample event");
        GlobeStatusText.Text = "Added a sample event near San Francisco.";
    }

    private void OnStartTourClick(object sender, RoutedEventArgs e)
    {
        Globe.StartTour();
        GlobeStatusText.Text = "Tour started.";
    }

    private void OnStopTourClick(object sender, RoutedEventArgs e)
    {
        Globe.StopTour();
        GlobeStatusText.Text = "Tour stopped.";
    }

    private void OnGlobeTourMarkerReached(object sender, ParticleGlobeMarkerEventArgs e)
    {
        GlobeStatusText.Text = $"Tour stop: {e.Marker.Label}";
    }

    private void OnGlobeTourEnded(object sender, EventArgs e)
    {
        GlobeStatusText.Text = "Tour ended.";
    }
}

public sealed partial class ParticleGlobeCountryOption(string name, string code)
{
    public string Name { get; } = name;
    public string Code { get; } = code;
}
