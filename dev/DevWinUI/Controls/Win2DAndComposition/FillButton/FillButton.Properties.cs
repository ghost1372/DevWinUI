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

    private static void OnToneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplyTone();

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplySize();

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplyIcon();

    private static void OnNavigateUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((FillButton)d).ApplyLinkBehavior();
}
