namespace DevWinUIGallery.Views;

public sealed partial class AntigravityPage : Page
{
    public ObservableCollection<AntigravityParticleShape> Shapes { get; set; } = new ObservableCollection<AntigravityParticleShape>(Enum.GetValues<AntigravityParticleShape>());
    public ObservableCollection<AntigravityColorMode> ColorModes { get; set; } = new ObservableCollection<AntigravityColorMode>(Enum.GetValues<AntigravityColorMode>());

    public AntigravityPage()
    {
        InitializeComponent();
    }

    private int ToInt(double value) => (int)Math.Round(value);

    private void Replay_Click(object sender, RoutedEventArgs e) => AntigravitySample.Replay();
}
