namespace DevWinUI;

public sealed partial class TextScribble
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextScribble),
            new PropertyMetadata("", (d, e) => ((TextScribble)d).OnTextChanged()));

    public string MarkText
    {
        get { return (string)GetValue(MarkTextProperty); }
        set { SetValue(MarkTextProperty, value); }
    }

    public static readonly DependencyProperty MarkTextProperty =
        DependencyProperty.Register(nameof(MarkText), typeof(string), typeof(TextScribble),
            new PropertyMetadata("", (d, e) => ((TextScribble)d).QueueRebuild()));

    public TextScribbleType Type
    {
        get { return (TextScribbleType)GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }

    public static readonly DependencyProperty TypeProperty =
        DependencyProperty.Register(nameof(Type), typeof(TextScribbleType), typeof(TextScribble),
            new PropertyMetadata(TextScribbleType.Scribble, (d, e) => ((TextScribble)d).QueueRebuild()));

    public Brush MarkBrush
    {
        get { return (Brush)GetValue(MarkBrushProperty); }
        set { SetValue(MarkBrushProperty, value); }
    }

    public static readonly DependencyProperty MarkBrushProperty =
        DependencyProperty.Register(nameof(MarkBrush), typeof(Brush), typeof(TextScribble),
            new PropertyMetadata(null, (d, e) => ((TextScribble)d).OnMarkBrushChanged()));

    public double StrokeWidth
    {
        get { return (double)GetValue(StrokeWidthProperty); }
        set { SetValue(StrokeWidthProperty, value); }
    }

    public static readonly DependencyProperty StrokeWidthProperty =
        DependencyProperty.Register(nameof(StrokeWidth), typeof(double), typeof(TextScribble),
            new PropertyMetadata(2.0, (d, e) => ((TextScribble)d).QueueRebuild()));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(TextScribble), new PropertyMetadata(0.9));

    public double Delay
    {
        get { return (double)GetValue(DelayProperty); }
        set { SetValue(DelayProperty, value); }
    }

    public static readonly DependencyProperty DelayProperty =
        DependencyProperty.Register(nameof(Delay), typeof(double), typeof(TextScribble), new PropertyMetadata(0.0));

    public TextScribbleTrigger Trigger
    {
        get { return (TextScribbleTrigger)GetValue(TriggerProperty); }
        set { SetValue(TriggerProperty, value); }
    }

    public static readonly DependencyProperty TriggerProperty =
        DependencyProperty.Register(nameof(Trigger), typeof(TextScribbleTrigger), typeof(TextScribble),
            new PropertyMetadata(TextScribbleTrigger.View, (d, e) => ((TextScribble)d).Evaluate()));

    public int? Iterations
    {
        get { return (int?)GetValue(IterationsProperty); }
        set { SetValue(IterationsProperty, value); }
    }

    public static readonly DependencyProperty IterationsProperty =
        DependencyProperty.Register(nameof(Iterations), typeof(int?), typeof(TextScribble),
            new PropertyMetadata(null, (d, e) => ((TextScribble)d).QueueRebuild()));

    public double MarkPadding
    {
        get { return (double)GetValue(MarkPaddingProperty); }
        set { SetValue(MarkPaddingProperty, value); }
    }

    public static readonly DependencyProperty MarkPaddingProperty =
        DependencyProperty.Register(nameof(MarkPadding), typeof(double), typeof(TextScribble),
            new PropertyMetadata(2.0, (d, e) => ((TextScribble)d).QueueRebuild()));

    public bool? Drawn
    {
        get { return (bool?)GetValue(DrawnProperty); }
        set { SetValue(DrawnProperty, value); }
    }

    public static readonly DependencyProperty DrawnProperty =
        DependencyProperty.Register(nameof(Drawn), typeof(bool?), typeof(TextScribble),
            new PropertyMetadata(null, (d, e) => ((TextScribble)d).Evaluate()));

    public TextWrapping TextWrapping
    {
        get { return (TextWrapping)GetValue(TextWrappingProperty); }
        set { SetValue(TextWrappingProperty, value); }
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(TextScribble),
            new PropertyMetadata(TextWrapping.Wrap, (d, e) => ((TextScribble)d).QueueRebuild()));
}
