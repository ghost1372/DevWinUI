namespace DevWinUI;

public sealed partial class UnderlineAnimation
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(UnderlineAnimation),
            new PropertyMetadata(string.Empty, (d, e) => ((UnderlineAnimation)d).OnTextChanged()));

    public string UnderlineText
    {
        get { return (string)GetValue(UnderlineTextProperty); }
        set { SetValue(UnderlineTextProperty, value); }
    }

    public static readonly DependencyProperty UnderlineTextProperty =
        DependencyProperty.Register(nameof(UnderlineText), typeof(string), typeof(UnderlineAnimation),
            new PropertyMetadata(string.Empty, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public UnderlineKind Kind
    {
        get { return (UnderlineKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(UnderlineKind), typeof(UnderlineAnimation),
            new PropertyMetadata(UnderlineKind.Slide, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public Uri? NavigateUri
    {
        get { return (Uri?)GetValue(NavigateUriProperty); }
        set { SetValue(NavigateUriProperty, value); }
    }

    public static readonly DependencyProperty NavigateUriProperty =
        DependencyProperty.Register(nameof(NavigateUri), typeof(Uri), typeof(UnderlineAnimation),
            new PropertyMetadata(null, (d, e) => ((UnderlineAnimation)d).UpdateInteractive()));

    public Brush? LineBrush
    {
        get { return (Brush?)GetValue(LineBrushProperty); }
        set { SetValue(LineBrushProperty, value); }
    }

    public static readonly DependencyProperty LineBrushProperty =
        DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(UnderlineAnimation),
            new PropertyMetadata(null, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public Brush? GradientBrush
    {
        get { return (Brush?)GetValue(GradientBrushProperty); }
        set { SetValue(GradientBrushProperty, value); }
    }

    public static readonly DependencyProperty GradientBrushProperty =
        DependencyProperty.Register(nameof(GradientBrush), typeof(Brush), typeof(UnderlineAnimation),
            new PropertyMetadata(null, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public double LineThickness
    {
        get { return (double)GetValue(LineThicknessProperty); }
        set { SetValue(LineThicknessProperty, value); }
    }

    public static readonly DependencyProperty LineThicknessProperty =
        DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(UnderlineAnimation),
            new PropertyMetadata(2.0, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public double LineOffset
    {
        get { return (double)GetValue(LineOffsetProperty); }
        set { SetValue(LineOffsetProperty, value); }
    }

    public static readonly DependencyProperty LineOffsetProperty =
        DependencyProperty.Register(nameof(LineOffset), typeof(double), typeof(UnderlineAnimation),
            new PropertyMetadata(2.0, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(UnderlineAnimation),
            new PropertyMetadata(420.0));

    public bool Rest
    {
        get { return (bool)GetValue(RestProperty); }
        set { SetValue(RestProperty, value); }
    }

    public static readonly DependencyProperty RestProperty =
        DependencyProperty.Register(nameof(Rest), typeof(bool), typeof(UnderlineAnimation),
            new PropertyMetadata(false, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));

    public bool Active
    {
        get { return (bool)GetValue(ActiveProperty); }
        set { SetValue(ActiveProperty, value); }
    }

    public static readonly DependencyProperty ActiveProperty =
        DependencyProperty.Register(nameof(Active), typeof(bool), typeof(UnderlineAnimation),
            new PropertyMetadata(false, (d, e) => ((UnderlineAnimation)d).OnActiveChanged()));

    public TextWrapping TextWrapping
    {
        get { return (TextWrapping)GetValue(TextWrappingProperty); }
        set { SetValue(TextWrappingProperty, value); }
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(UnderlineAnimation),
            new PropertyMetadata(TextWrapping.Wrap, (d, e) => ((UnderlineAnimation)d).QueueRebuild()));
}
