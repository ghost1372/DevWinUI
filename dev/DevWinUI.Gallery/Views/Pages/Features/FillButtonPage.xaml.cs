namespace DevWinUIGallery.Views;

public sealed partial class FillButtonPage : Page
{
    private static readonly Uri SampleUri = new Uri("https://github.com/Ghost1372/DevWinUI");

    public FillButtonPage()
    {
        InitializeComponent();
    }

    private object? GetIcon(bool isOn) => isOn ? new FontIcon { Glyph = "" } : null;

    private Uri? GetUri(bool isOn) => isOn ? SampleUri : null;

    private CommandBarLabelPosition GetSaveLabelPosition(bool collapsed) =>
        collapsed ? CommandBarLabelPosition.Collapsed : CommandBarLabelPosition.Default;

    private void Sample_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = $"Clicked at {DateTime.Now:T}";
    }

    private void CommandBarSample_Click(object sender, RoutedEventArgs e)
    {
        string name = sender switch
        {
            FillButton fill => fill.Label ?? string.Empty,
            AppBarButton bar => bar.Label,
            _ => sender.GetType().Name,
        };

        CommandStatusText.Text = $"{name} clicked at {DateTime.Now:T}";
    }
}
