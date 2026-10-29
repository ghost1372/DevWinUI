namespace DevWinUIGallery.Views;

public sealed partial class EyeTrackingPage : Page
{
    public ObservableCollection<EyeKind> KindItems { get; set; } = new ObservableCollection<EyeKind>(Enum.GetValues<EyeKind>());

    public EyeTrackingPage()
    {
        InitializeComponent();
    }

    private static int ToInt(double value) => (int)value;
}
