namespace DevWinUI;
public partial class HeaderCarousel
{
    public bool IsAutoScrollEnabled
    {
        get { return (bool)GetValue(IsAutoScrollEnabledProperty); }
        set { SetValue(IsAutoScrollEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAutoScrollEnabledProperty =
        DependencyProperty.Register(nameof(IsAutoScrollEnabled), typeof(bool), typeof(HeaderCarousel), new PropertyMetadata(true, OnIsAutoScrollEnabledChanged));

    private static void OnIsAutoScrollEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HeaderCarousel)d;
        if (ctl != null)
        {
            ctl.ApplyAutoScroll();
        }
    }

    public Stretch ImageStretch
    {
        get { return (Stretch)GetValue(ImageStretchProperty); }
        set { SetValue(ImageStretchProperty, value); }
    }

    public static readonly DependencyProperty ImageStretchProperty =
        DependencyProperty.Register(nameof(ImageStretch), typeof(Stretch), typeof(HeaderCarousel), new PropertyMetadata(Stretch.UniformToFill));

    public TimeSpan SelectionDuration
    {
        get { return (TimeSpan)GetValue(SelectionDurationProperty); }
        set { SetValue(SelectionDurationProperty, value); }
    }

    public static readonly DependencyProperty SelectionDurationProperty =
        DependencyProperty.Register(nameof(SelectionDuration), typeof(TimeSpan), typeof(HeaderCarousel), new PropertyMetadata(TimeSpan.FromSeconds(4), OnSelectionDurationChanged));

    private static void OnSelectionDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HeaderCarousel)d;
        if (ctl != null)
        {
            ctl.selectionTimer.Interval = (TimeSpan)e.NewValue;
        }
    }

    public TimeSpan DeSelectionDuration
    {
        get { return (TimeSpan)GetValue(DeSelectionDurationProperty); }
        set { SetValue(DeSelectionDurationProperty, value); }
    }

    public static readonly DependencyProperty DeSelectionDurationProperty =
        DependencyProperty.Register(nameof(DeSelectionDuration), typeof(TimeSpan), typeof(HeaderCarousel), new PropertyMetadata(TimeSpan.FromSeconds(3), OnDeSelectionDurationChanged));

    private static void OnDeSelectionDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HeaderCarousel)d;
        if (ctl != null)
        {
            ctl.deselectionTimer.Interval = (TimeSpan)e.NewValue;
        }
    }

    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(HeaderCarousel), new PropertyMetadata(null));

    public object SecondaryHeader
    {
        get { return (object)GetValue(SecondaryHeaderProperty); }
        set { SetValue(SecondaryHeaderProperty, value); }
    }

    public static readonly DependencyProperty SecondaryHeaderProperty =
        DependencyProperty.Register(nameof(SecondaryHeader), typeof(object), typeof(HeaderCarousel), new PropertyMetadata(null));

    public Visibility SecondaryHeaderVisibility
    {
        get { return (Visibility)GetValue(SecondaryHeaderVisibilityProperty); }
        set { SetValue(SecondaryHeaderVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SecondaryHeaderVisibilityProperty =
        DependencyProperty.Register(nameof(SecondaryHeaderVisibility), typeof(Visibility), typeof(HeaderCarousel), new PropertyMetadata(Visibility.Visible));

    public Visibility HeaderVisibility
    {
        get { return (Visibility)GetValue(HeaderVisibilityProperty); }
        set { SetValue(HeaderVisibilityProperty, value); }
    }

    public static readonly DependencyProperty HeaderVisibilityProperty =
        DependencyProperty.Register(nameof(HeaderVisibility), typeof(Visibility), typeof(HeaderCarousel), new PropertyMetadata(Visibility.Visible));

    public bool IsBlurEnabled
    {
        get { return (bool)GetValue(IsBlurBackgroundProperty); }
        set { SetValue(IsBlurBackgroundProperty, value); }
    }

    public static readonly DependencyProperty IsBlurBackgroundProperty =
        DependencyProperty.Register(nameof(IsBlurEnabled), typeof(bool), typeof(HeaderCarousel), new PropertyMetadata(true, IsBlurEnabledChanged));

    private static void IsBlurEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HeaderCarousel)d;
        ctl?.ApplyBackdropBlur();
    }

    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(HeaderCarousel), new PropertyMetadata(100.0, OnBlurChanged));

    private static void OnBlurChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HeaderCarousel)d;
        ctl?.ApplyBackdropBlur();
    }
}
