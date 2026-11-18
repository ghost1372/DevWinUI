namespace DevWinUI;

public sealed partial class FillButton
{
    public FillButtonTone Tone
    {
        get { return (FillButtonTone)GetValue(ToneProperty); }
        set { SetValue(ToneProperty, value); }
    }

    public static readonly DependencyProperty ToneProperty =
        DependencyProperty.Register(nameof(Tone), typeof(FillButtonTone), typeof(FillButton), new PropertyMetadata(FillButtonTone.Soft, OnToneChanged));

    public FillButtonSize Size
    {
        get { return (FillButtonSize)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(FillButtonSize), typeof(FillButton), new PropertyMetadata(FillButtonSize.Default, OnSizeChanged));

    public object? Icon
    {
        get { return GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(object), typeof(FillButton), new PropertyMetadata(null, OnIconChanged));

    public Uri? NavigateUri
    {
        get { return (Uri?)GetValue(NavigateUriProperty); }
        set { SetValue(NavigateUriProperty, value); }
    }

    public static readonly DependencyProperty NavigateUriProperty =
        DependencyProperty.Register(nameof(NavigateUri), typeof(Uri), typeof(FillButton), new PropertyMetadata(null, OnNavigateUriChanged));

    public string? Label
    {
        get { return (string?)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(FillButton), new PropertyMetadata(null, OnLabelChanged));

    public CommandBarLabelPosition LabelPosition
    {
        get { return (CommandBarLabelPosition)GetValue(LabelPositionProperty); }
        set { SetValue(LabelPositionProperty, value); }
    }

    public static readonly DependencyProperty LabelPositionProperty =
        DependencyProperty.Register(nameof(LabelPosition), typeof(CommandBarLabelPosition), typeof(FillButton), new PropertyMetadata(CommandBarLabelPosition.Default, OnLabelVisibilityChanged));

    public bool IsCompact
    {
        get { return (bool)GetValue(IsCompactProperty); }
        set { SetValue(IsCompactProperty, value); }
    }

    public static readonly DependencyProperty IsCompactProperty =
        DependencyProperty.Register(nameof(IsCompact), typeof(bool), typeof(FillButton), new PropertyMetadata(false, OnLabelVisibilityChanged));

    public bool IsInOverflow
    {
        get { return (bool)GetValue(IsInOverflowProperty); }
        private set { SetValue(IsInOverflowProperty, value); }
    }

    public static readonly DependencyProperty IsInOverflowProperty =
        DependencyProperty.Register(nameof(IsInOverflow), typeof(bool), typeof(FillButton), new PropertyMetadata(false, OnLabelVisibilityChanged));

    public int DynamicOverflowOrder
    {
        get { return (int)GetValue(DynamicOverflowOrderProperty); }
        set { SetValue(DynamicOverflowOrderProperty, value); }
    }

    public static readonly DependencyProperty DynamicOverflowOrderProperty =
        DependencyProperty.Register(nameof(DynamicOverflowOrder), typeof(int), typeof(FillButton), new PropertyMetadata(0));

    private static void OnToneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplyTone();

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplySize();

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplyIcon();

    private static void OnNavigateUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplyLinkBehavior();

    private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).Content = e.NewValue;

    private static void OnLabelVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).UpdateLabelVisibility();
}
