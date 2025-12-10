namespace DevWinUIGallery.Views;

public sealed partial class SpectrumVisualizerPage : Page
{
    public BaseViewModel ViewModel { get; }
    public SpectrumVisualizerPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();

        SpectrumVisualizerSample.Analyzer = new NaudioSpectrumAnalyzer();
        SpectrumVisualizerSample.CustomColorProvider = (intensity, i) =>
        {
            return i % 2 == 0 ? Colors.HotPink : Colors.Cyan;
        };
    }
}
