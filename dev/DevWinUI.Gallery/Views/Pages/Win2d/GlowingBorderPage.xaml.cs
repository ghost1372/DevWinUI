namespace DevWinUIGallery.Views;

public sealed partial class GlowingBorderPage : Page
{
    public ObservableCollection<GlowingBorderKind> KindItems { get; set; } = new ObservableCollection<GlowingBorderKind>(Enum.GetValues<GlowingBorderKind>());

    public GlowingBorderPage()
    {
        InitializeComponent();
    }
}
