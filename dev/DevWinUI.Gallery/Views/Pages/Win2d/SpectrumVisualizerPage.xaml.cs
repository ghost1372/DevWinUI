namespace DevWinUIGallery.Views;

public sealed partial class SpectrumVisualizerPage : Page
{
    public ObservableCollection<SpectrumColorType> SpectrumColorTypeList { get; set; } = new ObservableCollection<SpectrumColorType>(Enum.GetValues<SpectrumColorType>());
    public ObservableCollection<SpectrumType> SpectrumTypeList { get; set; } = new ObservableCollection<SpectrumType>(Enum.GetValues<SpectrumType>());

    public SpectrumVisualizerPage()
    {
        InitializeComponent();

        SpectrumVisualizerSample.Analyzer = new AudioGraphSpectrumAnalyzer();
        SpectrumVisualizerSample.CustomColorProvider = (intensity, i) =>
        {
            return i % 2 == 0 ? Colors.HotPink : Colors.Cyan;
        };
    }
}
