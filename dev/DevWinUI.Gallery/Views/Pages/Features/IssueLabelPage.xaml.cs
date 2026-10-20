namespace DevWinUIGallery.Views;
public sealed partial class IssueLabelPage : Page
{
    public ObservableCollection<IssueLabelSeverity> SeverityItems { get; } = new ObservableCollection<IssueLabelSeverity>(Enum.GetValues<IssueLabelSeverity>());

    public IssueLabelPage()
    {
        this.InitializeComponent();
    }
}
