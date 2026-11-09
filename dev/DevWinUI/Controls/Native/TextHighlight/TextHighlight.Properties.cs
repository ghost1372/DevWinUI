namespace DevWinUI;

public sealed partial class TextHighlight
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextHighlight),
            new PropertyMetadata("", (d, e) => ((TextHighlight)d).OnTextChanged()));

    public string HighlightText
    {
        get { return (string)GetValue(HighlightTextProperty); }
        set { SetValue(HighlightTextProperty, value); }
    }

    public static readonly DependencyProperty HighlightTextProperty =
        DependencyProperty.Register(nameof(HighlightText), typeof(string), typeof(TextHighlight),
            new PropertyMetadata("", (d, e) => ((TextHighlight)d).QueueRebuild()));

    public Brush HighlightBrush
    {
        get { return (Brush)GetValue(HighlightBrushProperty); }
        set { SetValue(HighlightBrushProperty, value); }
    }

    public static readonly DependencyProperty HighlightBrushProperty =
        DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(TextHighlight),
            new PropertyMetadata(null, (d, e) => ((TextHighlight)d).OnHighlightBrushChanged()));

    public double BandOpacity
    {
        get { return (double)GetValue(BandOpacityProperty); }
        set { SetValue(BandOpacityProperty, value); }
    }

    public static readonly DependencyProperty BandOpacityProperty =
        DependencyProperty.Register(nameof(BandOpacity), typeof(double), typeof(TextHighlight),
            new PropertyMetadata(0.4, (d, e) => ((TextHighlight)d).OnBandOpacityChanged()));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(TextHighlight), new PropertyMetadata(0.9));

    public double Delay
    {
        get { return (double)GetValue(DelayProperty); }
        set { SetValue(DelayProperty, value); }
    }

    public static readonly DependencyProperty DelayProperty =
        DependencyProperty.Register(nameof(Delay), typeof(double), typeof(TextHighlight), new PropertyMetadata(0.0));

    public TextHighlightTrigger Trigger
    {
        get { return (TextHighlightTrigger)GetValue(TriggerProperty); }
        set { SetValue(TriggerProperty, value); }
    }

    public static readonly DependencyProperty TriggerProperty =
        DependencyProperty.Register(nameof(Trigger), typeof(TextHighlightTrigger), typeof(TextHighlight),
            new PropertyMetadata(TextHighlightTrigger.View, (d, e) => ((TextHighlight)d).Evaluate()));

    public double BandPadding
    {
        get { return (double)GetValue(BandPaddingProperty); }
        set { SetValue(BandPaddingProperty, value); }
    }

    public static readonly DependencyProperty BandPaddingProperty =
        DependencyProperty.Register(nameof(BandPadding), typeof(double), typeof(TextHighlight),
            new PropertyMetadata(2.0, (d, e) => ((TextHighlight)d).QueueRebuild()));

    public double BandCornerRadius
    {
        get { return (double)GetValue(BandCornerRadiusProperty); }
        set { SetValue(BandCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty BandCornerRadiusProperty =
        DependencyProperty.Register(nameof(BandCornerRadius), typeof(double), typeof(TextHighlight),
            new PropertyMetadata(4.0, (d, e) => ((TextHighlight)d).OnBandCornerRadiusChanged()));

    public bool? Drawn
    {
        get { return (bool?)GetValue(DrawnProperty); }
        set { SetValue(DrawnProperty, value); }
    }

    public static readonly DependencyProperty DrawnProperty =
        DependencyProperty.Register(nameof(Drawn), typeof(bool?), typeof(TextHighlight),
            new PropertyMetadata(null, (d, e) => ((TextHighlight)d).Evaluate()));

    public TextWrapping TextWrapping
    {
        get { return (TextWrapping)GetValue(TextWrappingProperty); }
        set { SetValue(TextWrappingProperty, value); }
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(TextHighlight),
            new PropertyMetadata(TextWrapping.Wrap, (d, e) => ((TextHighlight)d).QueueRebuild()));
}
