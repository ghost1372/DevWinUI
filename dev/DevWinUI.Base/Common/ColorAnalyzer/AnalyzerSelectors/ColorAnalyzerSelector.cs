namespace DevWinUI;

public abstract partial class ColorAnalyzerSelector : DependencyObject
{
    private IEnumerable<AnalyzedColor>? _analyzedColor;

    public static readonly DependencyProperty SelectedColorsProperty =
        DependencyProperty.Register(
            nameof(SelectedColors),
            typeof(IList<Color>),
            typeof(ColorAnalyzerSelector),
            new PropertyMetadata(null));

    public static readonly DependencyProperty MinColorCountProperty =
        DependencyProperty.Register(
            nameof(MinColorCount),
            typeof(int),
            typeof(ColorAnalyzerSelector),
            new PropertyMetadata(1, OnMinColorCountChanged));

    public IList<Color>? SelectedColors
    {
        get => (IList<Color>?)GetValue(SelectedColorsProperty);
        protected set => SetValue(SelectedColorsProperty, value);
    }

    public int MinColorCount
    {
        get => (int)GetValue(MinColorCountProperty);
        set => SetValue(MinColorCountProperty, value);
    }

    public virtual void SelectColors(IEnumerable<AnalyzedColor> analyzedColor)
    {
        _analyzedColor = analyzedColor;
    }

    private static void OnMinColorCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ColorAnalyzerSelector selector || selector._analyzedColor is null)
            return;

        selector.SelectColors(selector._analyzedColor);
    }
}
