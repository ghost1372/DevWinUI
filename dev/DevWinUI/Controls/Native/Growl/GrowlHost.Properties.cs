namespace DevWinUI;

public sealed partial class GrowlHost
{
    public string HostKey
    {
        get { return (string)GetValue(HostKeyProperty); }
        set { SetValue(HostKeyProperty, value); }
    }

    public static readonly DependencyProperty HostKeyProperty =
        DependencyProperty.Register(nameof(HostKey), typeof(string), typeof(GrowlHost), new PropertyMetadata(string.Empty, OnHostKeyChanged));

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(GrowlHost), new PropertyMetadata(8d, OnLayoutMetricsChanged));

    public int MaxVisiblePerPosition
    {
        get { return (int)GetValue(MaxVisiblePerPositionProperty); }
        set { SetValue(MaxVisiblePerPositionProperty, value); }
    }

    public static readonly DependencyProperty MaxVisiblePerPositionProperty =
        DependencyProperty.Register(nameof(MaxVisiblePerPosition), typeof(int), typeof(GrowlHost), new PropertyMetadata(0, OnMaxVisibleChanged));

    public GrowlPosition DefaultPosition
    {
        get { return (GrowlPosition)GetValue(DefaultPositionProperty); }
        set { SetValue(DefaultPositionProperty, value); }
    }

    public static readonly DependencyProperty DefaultPositionProperty =
        DependencyProperty.Register(nameof(DefaultPosition), typeof(GrowlPosition), typeof(GrowlHost), new PropertyMetadata(GrowlPosition.TopRight));

    public GrowlHostLayoutMode LayoutMode
    {
        get { return (GrowlHostLayoutMode)GetValue(LayoutModeProperty); }
        set { SetValue(LayoutModeProperty, value); }
    }

    public static readonly DependencyProperty LayoutModeProperty =
        DependencyProperty.Register(nameof(LayoutMode), typeof(GrowlHostLayoutMode), typeof(GrowlHost), new PropertyMetadata(GrowlHostLayoutMode.List, OnLayoutModeChanged));

    public double StackPeekHeight
    {
        get { return (double)GetValue(StackPeekHeightProperty); }
        set { SetValue(StackPeekHeightProperty, value); }
    }

    public static readonly DependencyProperty StackPeekHeightProperty =
        DependencyProperty.Register(nameof(StackPeekHeight), typeof(double), typeof(GrowlHost), new PropertyMetadata(14d, OnLayoutMetricsChanged));

    public int StackVisibleDepth
    {
        get { return (int)GetValue(StackVisibleDepthProperty); }
        set { SetValue(StackVisibleDepthProperty, value); }
    }

    public static readonly DependencyProperty StackVisibleDepthProperty =
        DependencyProperty.Register(nameof(StackVisibleDepth), typeof(int), typeof(GrowlHost), new PropertyMetadata(3, OnLayoutMetricsChanged));

    public bool StackExpandOnHover
    {
        get { return (bool)GetValue(StackExpandOnHoverProperty); }
        set { SetValue(StackExpandOnHoverProperty, value); }
    }

    public static readonly DependencyProperty StackExpandOnHoverProperty =
        DependencyProperty.Register(nameof(StackExpandOnHover), typeof(bool), typeof(GrowlHost), new PropertyMetadata(true, OnStackExpandOnHoverChanged));

    public bool NewestOnTop
    {
        get { return (bool)GetValue(NewestOnTopProperty); }
        set { SetValue(NewestOnTopProperty, value); }
    }

    public static readonly DependencyProperty NewestOnTopProperty =
        DependencyProperty.Register(nameof(NewestOnTop), typeof(bool), typeof(GrowlHost), new PropertyMetadata(false, OnLayoutModeChanged));

    public GrowlShowAnimation DefaultShowAnimation
    {
        get { return (GrowlShowAnimation)GetValue(DefaultShowAnimationProperty); }
        set { SetValue(DefaultShowAnimationProperty, value); }
    }

    public static readonly DependencyProperty DefaultShowAnimationProperty =
        DependencyProperty.Register(nameof(DefaultShowAnimation), typeof(GrowlShowAnimation), typeof(GrowlHost), new PropertyMetadata(GrowlShowAnimation.Slide));

    public GrowlCloseAnimation DefaultCloseAnimation
    {
        get { return (GrowlCloseAnimation)GetValue(DefaultCloseAnimationProperty); }
        set { SetValue(DefaultCloseAnimationProperty, value); }
    }

    public static readonly DependencyProperty DefaultCloseAnimationProperty =
        DependencyProperty.Register(nameof(DefaultCloseAnimation), typeof(GrowlCloseAnimation), typeof(GrowlHost), new PropertyMetadata(GrowlCloseAnimation.Fade));

    public TimeSpan AnimationDuration
    {
        get { return (TimeSpan)GetValue(AnimationDurationProperty); }
        set { SetValue(AnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty AnimationDurationProperty =
        DependencyProperty.Register(nameof(AnimationDuration), typeof(TimeSpan), typeof(GrowlHost), new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

    public GrowlCardColorStyle DefaultColorStyle
    {
        get { return (GrowlCardColorStyle)GetValue(DefaultColorStyleProperty); }
        set { SetValue(DefaultColorStyleProperty, value); }
    }

    public static readonly DependencyProperty DefaultColorStyleProperty =
        DependencyProperty.Register(nameof(DefaultColorStyle), typeof(GrowlCardColorStyle), typeof(GrowlHost), new PropertyMetadata(GrowlCardColorStyle.Solid));

    public ScrollBarVisibility ScrollBarVisibility
    {
        get { return (ScrollBarVisibility)GetValue(ScrollBarVisibilityProperty); }
        set { SetValue(ScrollBarVisibilityProperty, value); }
    }

    public static readonly DependencyProperty ScrollBarVisibilityProperty =
        DependencyProperty.Register(nameof(ScrollBarVisibility), typeof(ScrollBarVisibility), typeof(GrowlHost), new PropertyMetadata(ScrollBarVisibility.Auto));

    public GrowlSound DefaultSound
    {
        get { return (GrowlSound)GetValue(DefaultSoundProperty); }
        set { SetValue(DefaultSoundProperty, value); }
    }

    public static readonly DependencyProperty DefaultSoundProperty =
        DependencyProperty.Register(nameof(DefaultSound), typeof(GrowlSound), typeof(GrowlHost), new PropertyMetadata(GrowlSound.None));

    private static void OnStackExpandOnHoverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (GrowlHost)d;

        host._expanded.Clear();

        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            host.ApplyLayout(position);
        }
    }

    private static void OnMaxVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (GrowlHost)d;

        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            host.Pump(position);
        }
    }

    private static void OnHostKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Growl.UpdateKey((GrowlHost)d, e.NewValue as string ?? string.Empty);
    }

    private static void OnLayoutModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (GrowlHost)d;

        host._expanded.Clear();

        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            host.Reorder(position);
            host.ApplyLayout(position);
        }
    }

    private static void OnLayoutMetricsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (GrowlHost)d;

        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            host.ApplyLayout(position);
        }
    }

}
