namespace DevWinUIGallery.Views;

public sealed partial class ColorArchiveScrollPage : Page
{
    public ObservableCollection<ColorArchivePreviewSize> PreviewSizeItems { get; set; } = new ObservableCollection<ColorArchivePreviewSize>(Enum.GetValues<ColorArchivePreviewSize>());

    public ObservableCollection<ColorArchiveProject> Projects { get; } = new()
    {
        ColorArchiveProject.Make("aurora", "Aurora Identity", "Branding", "2026", "#F4E4D4"),
        ColorArchiveProject.Make("lumen", "Lumen Dashboard", "Product", "2026", "#DCE8F7"),
        ColorArchiveProject.Make("verdant", "Verdant Market", "E-commerce", "2025", "#DDEBD8"),
        ColorArchiveProject.Make("ember", "Ember Studio", "Web", "2025", "#F6D5CB"),
        ColorArchiveProject.Make("nocturne", "Nocturne Audio", "Mobile", "2025", "#1F1F2E"),
        ColorArchiveProject.Make("solstice", "Solstice Journal", "Editorial", "2024", "#F7EBB5"),
        ColorArchiveProject.Make("tidal", "Tidal Weather", "Mobile", "2024", "#C9E7EA"),
        ColorArchiveProject.Make("orchid", "Orchid Wellness", "Branding", "2024", "#E7D7EE"),
    };

    public ColorArchiveScrollPage()
    {
        InitializeComponent();
    }

    private void OnActiveChanged(object sender, ColorArchiveActiveChangedEventArgs e)
    {
        Archive.ActiveIndex = e.Index;
        CmbProject.SelectedIndex = e.Index;
        TxtActive.Text = $"{e.Index + 1}. {e.Project.Name} - {e.Project.Badge}, {e.Project.Date}";
    }

    private void OnProjectSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Archive is null || CmbProject.SelectedIndex < 0)
        {
            return;
        }

        Archive.ActiveIndex = CmbProject.SelectedIndex;
    }
}
