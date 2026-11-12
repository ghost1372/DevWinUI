namespace DevWinUIGallery.Views;

public sealed partial class DiagramPage : Page
{
    public ObservableCollection<DiagramLayout> LayoutItems { get; } = new(Enum.GetValues<DiagramLayout>());
    public ObservableCollection<DiagramDirection> DirectionItems { get; } = new(Enum.GetValues<DiagramDirection>());
    public ObservableCollection<DiagramEdgeStyle> EdgeStyleItems { get; } = new(Enum.GetValues<DiagramEdgeStyle>());
    public ObservableCollection<DiagramBackground> BackgroundItems { get; } = new(Enum.GetValues<DiagramBackground>());
    public ObservableCollection<DiagramSelectionMode> SelectionModeItems { get; } = new(Enum.GetValues<DiagramSelectionMode>());
    public ObservableCollection<DiagramPinShape> PinShapeItems { get; } = new(Enum.GetValues<DiagramPinShape>());

    public DiagramPage()
    {
        InitializeComponent();
    }
}
