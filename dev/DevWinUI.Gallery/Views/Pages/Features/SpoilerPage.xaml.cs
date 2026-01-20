namespace DevWinUIGallery.Views;

public sealed partial class SpoilerPage : Page
{
    public ObservableCollection<SpoilerRevealMode> RevealModeItems { get; set; } = new ObservableCollection<SpoilerRevealMode>(Enum.GetValues<SpoilerRevealMode>());
    public ObservableCollection<VerticalAlignment> VerticalAlignmentItems { get; set; } = new ObservableCollection<VerticalAlignment>(Enum.GetValues<VerticalAlignment>());
    public ObservableCollection<HorizontalAlignment> HorizontalAlignmentItems { get; set; } = new ObservableCollection<HorizontalAlignment>(Enum.GetValues<HorizontalAlignment>());

    public SpoilerPage()
    {
        InitializeComponent();
    }
}
