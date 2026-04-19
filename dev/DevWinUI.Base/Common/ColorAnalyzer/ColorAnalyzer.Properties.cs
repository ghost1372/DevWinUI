namespace DevWinUI;

public partial class ColorAnalyzer
{
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(UIElement), typeof(ColorAnalyzer), new PropertyMetadata(null, OnSourceChanged));

    public event EventHandler? AnalyzerUpdated;

    public UIElement? Source
    {
        get => (UIElement)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public IList<ColorAnalyzerSelector> Analyzers { get; set; }

    public IReadOnlyList<AnalyzedColor>? AnalyzedColors { get; private set; }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ColorAnalyzer analyzer)
            return;

        _ = analyzer.UpdateAnalyzerAsync();
    }
}
