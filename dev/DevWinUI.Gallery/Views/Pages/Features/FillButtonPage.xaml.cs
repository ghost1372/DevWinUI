namespace DevWinUIGallery.Views;

public sealed partial class FillButtonPage : Page
{
    private static readonly Uri SampleUri = new Uri("https://github.com/Ghost1372/DevWinUI");

    public ObservableCollection<FillButtonTone> ToneItems { get; set; } = new ObservableCollection<FillButtonTone>(Enum.GetValues<FillButtonTone>());
    public ObservableCollection<FillButtonSize> SizeItems { get; set; } = new ObservableCollection<FillButtonSize>(Enum.GetValues<FillButtonSize>());

    public FillButtonPage()
    {
        InitializeComponent();
    }

    private object? GetIcon(bool isOn) => isOn ? new FontIcon { Glyph = "\uE710" } : null;

    private Uri? GetUri(bool isOn) => isOn ? SampleUri : null;

    private void Sample_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = $"Clicked at {DateTime.Now:T}";
    }
}
