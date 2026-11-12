namespace DevWinUIGallery.Views;

public sealed partial class SpectrumVisualizerPage : Page
{
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
