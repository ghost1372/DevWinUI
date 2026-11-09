namespace DevWinUIGallery.Views;

public sealed partial class SegmentedMeterPage : Page
{
    public ObservableCollection<MeterBarSize> BarSizeItems { get; set; } = new ObservableCollection<MeterBarSize>(Enum.GetValues<MeterBarSize>());
    public ObservableCollection<MeterLegendPosition> LegendPositionItems { get; set; } = new ObservableCollection<MeterLegendPosition>(Enum.GetValues<MeterLegendPosition>());
    public ObservableCollection<Orientation> OrientationItems { get; set; } = new ObservableCollection<Orientation>(Enum.GetValues<Orientation>());

    public SegmentedMeterPage()
    {
        InitializeComponent();
    }
}
