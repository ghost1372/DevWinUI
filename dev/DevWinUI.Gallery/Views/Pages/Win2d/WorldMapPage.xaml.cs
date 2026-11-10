namespace DevWinUIGallery.Views;

public sealed partial class WorldMapPage : Page
{
    public ObservableCollection<WorldMapSelectionMode> SelectionModeItems { get; } = new(Enum.GetValues<WorldMapSelectionMode>());
    public ObservableCollection<WorldMapHighlightColor> HighlightColorItems { get; } = new(Enum.GetValues<WorldMapHighlightColor>());
    public ObservableCollection<WorldMapBorderStyle> BorderStyleItems { get; } = new(Enum.GetValues<WorldMapBorderStyle>());

    public WorldMapPage()
    {
        InitializeComponent();
    }

    private void OnMapLoaded(object sender, EventArgs e)
    {
        MapStatusText.Text = $"{Map.Countries.Count} countries loaded. Select a country to explore.";
    }

    private void OnMapCountryClicked(object sender, WorldMapCountryEventArgs e)
    {
        MapStatusText.Text = e.Country is null
            ? $"{Map.SelectedCodes.Count} countries selected."
            : $"Clicked: {e.Country.Name} ({e.Country.Code}).";
    }

    private void OnMapSelectionChanged(object sender, WorldMapSelectionChangedEventArgs e)
    {
        MapStatusText.Text = $"{Map.SelectedCodes.Count} countries selected.";
    }
}
