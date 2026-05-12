namespace DevWinUI;

public partial class LiveGraph
{
    public HighlightLineBehavior HighlightLineBehavior
    {
        get { return (HighlightLineBehavior)GetValue(HighlightLineBehaviorProperty); }
        set { SetValue(HighlightLineBehaviorProperty, value); }
    }

    public static readonly DependencyProperty HighlightLineBehaviorProperty =
        DependencyProperty.Register(nameof(HighlightLineBehavior), typeof(HighlightLineBehavior), typeof(LiveGraph), new PropertyMetadata(HighlightLineBehavior.EachPoint, OnHighlightLineBehaviorChanged));

    private static void OnHighlightLineBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.highlightLineBehavior = (HighlightLineBehavior)e.NewValue;
    }

    public Visibility HighlightLineVisibility
    {
        get { return (Visibility)GetValue(HighlightLineVisibilityProperty); }
        set { SetValue(HighlightLineVisibilityProperty, value); }
    }

    public static readonly DependencyProperty HighlightLineVisibilityProperty =
        DependencyProperty.Register(nameof(HighlightLineVisibility), typeof(Visibility), typeof(LiveGraph), new PropertyMetadata(Visibility.Visible, OnHighlightLineVisibilityChanged));

    private static void OnHighlightLineVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.highlightLineVisibility = (Visibility)e.NewValue;
    }

    public LiveGraphBackgroundMode BackgroundMode
    {
        get { return (LiveGraphBackgroundMode)GetValue(BackgroundModeProperty); }
        set { SetValue(BackgroundModeProperty, value); }
    }

    public static readonly DependencyProperty BackgroundModeProperty =
        DependencyProperty.Register(nameof(BackgroundMode), typeof(LiveGraphBackgroundMode), typeof(LiveGraph), new PropertyMetadata(LiveGraphBackgroundMode.Cross, OnBackgroundModeChanged));

    private static void OnBackgroundModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.backgroundMode = (LiveGraphBackgroundMode)e.NewValue;
    }

    public double DotSpacing
    {
        get { return (double)GetValue(DotSpacingProperty); }
        set { SetValue(DotSpacingProperty, value); }
    }

    public static readonly DependencyProperty DotSpacingProperty =
        DependencyProperty.Register(nameof(DotSpacing), typeof(double), typeof(LiveGraph), new PropertyMetadata(6.0, OnDotSpacingChanged));

    private static void OnDotSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.dotSpacing = Convert.ToSingle(e.NewValue);
    }

    public double CrossSpacing
    {
        get { return (double)GetValue(CrossSpacingProperty); }
        set { SetValue(CrossSpacingProperty, value); }
    }

    public static readonly DependencyProperty CrossSpacingProperty =
        DependencyProperty.Register(nameof(CrossSpacing), typeof(double), typeof(LiveGraph), new PropertyMetadata(30.0, OnCrossSpacingChanged));

    private static void OnCrossSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.crossSpacing = Convert.ToSingle(e.NewValue);
    }

    public Color? BackgroundColor
    {
        get { return (Color?)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    public static readonly DependencyProperty BackgroundColorProperty =
        DependencyProperty.Register(nameof(BackgroundColor), typeof(Color?), typeof(LiveGraph), new PropertyMetadata(null, OnBackgroundColorChanged));
    private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        if (e.NewValue != null)
        {
            ctl.drawColor = (Color)e.NewValue;
        }
        else
        {
            ctl.UpdateDrawColor();
        }
    }

    public Color? ClearColor
    {
        get { return (Color?)GetValue(ClearColorProperty); }
        set { SetValue(ClearColorProperty, value); }
    }

    public static readonly DependencyProperty ClearColorProperty =
        DependencyProperty.Register(nameof(ClearColor), typeof(Color?), typeof(LiveGraph), new PropertyMetadata(Colors.Transparent, OnClearColorChanged));

    private static void OnClearColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;

        ctl.clearColor = e.NewValue != null ? (Color)e.NewValue : Colors.Transparent;
        ctl.UpdateClearColor();

    }

    public double HorizontalScrollDistance
    {
        get { return (double)GetValue(HorizontalScrollDistanceProperty); }
        set { SetValue(HorizontalScrollDistanceProperty, value); }
    }

    public static readonly DependencyProperty HorizontalScrollDistanceProperty =
        DependencyProperty.Register(nameof(HorizontalScrollDistance), typeof(double), typeof(LiveGraph), new PropertyMetadata(1.0d, OnHorizontalScrollChanged));

    public TimeSpan HorizontalScrollDuration
    {
        get { return (TimeSpan)GetValue(HorizontalScrollDurationProperty); }
        set { SetValue(HorizontalScrollDurationProperty, value); }
    }

    public static readonly DependencyProperty HorizontalScrollDurationProperty =
        DependencyProperty.Register(nameof(HorizontalScrollDuration), typeof(TimeSpan), typeof(LiveGraph), new PropertyMetadata(TimeSpan.FromSeconds(1), OnHorizontalScrollChanged));

    private static void OnHorizontalScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.UpdateHorizontalScrollSpeed();
    }

    private void UpdateHorizontalScrollSpeed()
    {
        horizontalScrollSpeed = CalculateSpeed((float)HorizontalScrollDistance, HorizontalScrollDuration);
    }

    public object HighlightLineContent
    {
        get { return (object)GetValue(HighlightLineContentProperty); }
        set { SetValue(HighlightLineContentProperty, value); }
    }

    public static readonly DependencyProperty HighlightLineContentProperty =
        DependencyProperty.Register(nameof(HighlightLineContent), typeof(object), typeof(LiveGraph), new PropertyMetadata(null, OnHighlightLineContentChanged));

    private static void OnHighlightLineContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LiveGraph)d;
        ctl.highlightLineContent = e.NewValue;
    }
}
