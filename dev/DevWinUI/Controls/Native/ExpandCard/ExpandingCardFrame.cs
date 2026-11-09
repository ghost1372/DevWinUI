namespace DevWinUI;

[TemplatePart(Name = RootPartName, Type = typeof(Border))]
public sealed partial class ExpandingCardFrame : Control
{
    private const string RootPartName = "RootBorder";

    private Border? _root;

    public ExpandingCardFrame()
    {
        DefaultStyleKey = typeof(ExpandingCardFrame);
    }

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ExpandingCardFrame), new PropertyMetadata(""));

    public ImageSource? ImageSource
    {
        get { return (ImageSource?)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ExpandingCardFrame), new PropertyMetadata(null, OnImageSourceChanged));

    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(ExpandingCardFrame), new PropertyMetadata(0.0, OnProgressChanged));

    public double TitleOpacity
    {
        get { return (double)GetValue(TitleOpacityProperty); }
        private set { SetValue(TitleOpacityProperty, value); }
    }

    public static readonly DependencyProperty TitleOpacityProperty =
        DependencyProperty.Register(nameof(TitleOpacity), typeof(double), typeof(ExpandingCardFrame), new PropertyMetadata(0.0));

    public bool ShowFocusRing
    {
        get { return (bool)GetValue(ShowFocusRingProperty); }
        set { SetValue(ShowFocusRingProperty, value); }
    }

    public static readonly DependencyProperty ShowFocusRingProperty =
        DependencyProperty.Register(nameof(ShowFocusRing), typeof(bool), typeof(ExpandingCardFrame), new PropertyMetadata(false, OnShowFocusRingChanged));

    public Brush? ScrimBrush
    {
        get { return (Brush?)GetValue(ScrimBrushProperty); }
        set { SetValue(ScrimBrushProperty, value); }
    }

    public static readonly DependencyProperty ScrimBrushProperty =
        DependencyProperty.Register(nameof(ScrimBrush), typeof(Brush), typeof(ExpandingCardFrame), new PropertyMetadata(null));

    public Brush? FocusBrush
    {
        get { return (Brush?)GetValue(FocusBrushProperty); }
        set { SetValue(FocusBrushProperty, value); }
    }

    public static readonly DependencyProperty FocusBrushProperty =
        DependencyProperty.Register(nameof(FocusBrush), typeof(Brush), typeof(ExpandingCardFrame), new PropertyMetadata(null));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _root = GetTemplateChild(RootPartName) as Border;
        ApplyImage();
        UpdateVisualState(false);
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ExpandingCardFrame)d).ApplyImage();

    private void ApplyImage()
    {
        if (_root is null) return;
        _root.Background = ImageSource is null
            ? null
            : new ImageBrush { ImageSource = ImageSource, Stretch = Stretch.UniformToFill };
    }

    private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        double p = (double)e.NewValue;
        ((ExpandingCardFrame)d).TitleOpacity = p * p;
    }

    private static void OnShowFocusRingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ExpandingCardFrame)d).UpdateVisualState(true);

    private void UpdateVisualState(bool useTransitions) =>
        VisualStateManager.GoToState(this, ShowFocusRing ? "FocusVisible" : "FocusHidden", useTransitions);
}
