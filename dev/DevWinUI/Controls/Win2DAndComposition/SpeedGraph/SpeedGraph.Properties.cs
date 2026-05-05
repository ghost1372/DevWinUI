namespace DevWinUI;

public partial class SpeedGraph
{
    public SpeedGraphMode Mode
    {
        get { return (SpeedGraphMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(SpeedGraphMode), typeof(SpeedGraph), new PropertyMetadata(SpeedGraphMode.StaticProgress, OnModeChanged));

    private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpeedGraph)d;
        if (ctl != null)
        {
            ctl.UpdateMode();
        }
    }

    public SpeedGraphBackgroundMode BackgroundMode
    {
        get { return (SpeedGraphBackgroundMode)GetValue(BackgroundModeProperty); }
        set { SetValue(BackgroundModeProperty, value); }
    }

    public static readonly DependencyProperty BackgroundModeProperty =
        DependencyProperty.Register(nameof(BackgroundMode), typeof(SpeedGraphBackgroundMode), typeof(SpeedGraph), new PropertyMetadata(SpeedGraphBackgroundMode.Dot, OnBackgroundStyleChanged));

    private static void OnBackgroundStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpeedGraph)d;
        if (ctl != null)
        {
            ctl.UpdateBackgroundShape();
        }
    }

    public int BackgroundShapeDistance
    {
        get { return (int)GetValue(BackgroundShapeDistanceProperty); }
        set { SetValue(BackgroundShapeDistanceProperty, value); }
    }

    public static readonly DependencyProperty BackgroundShapeDistanceProperty =
        DependencyProperty.Register(nameof(BackgroundShapeDistance), typeof(int), typeof(SpeedGraph), new PropertyMetadata(6, OnBackgroundShapeDistanceChanged));

    private static void OnBackgroundShapeDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpeedGraph)d;
        if (ctl != null)
        {
            ctl.canvas?.Invalidate();
        }
    }

    public Visibility SpeedLineVisibility
    {
        get { return (Visibility)GetValue(SpeedLineVisibilityProperty); }
        set { SetValue(SpeedLineVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SpeedLineVisibilityProperty =
        DependencyProperty.Register(nameof(SpeedLineVisibility), typeof(Visibility), typeof(SpeedGraph), new PropertyMetadata(Visibility.Visible));

    public Visibility SpeedTextVisibility
    {
        get { return (Visibility)GetValue(SpeedTextVisibilityProperty); }
        set { SetValue(SpeedTextVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SpeedTextVisibilityProperty =
        DependencyProperty.Register(nameof(SpeedTextVisibility), typeof(Visibility), typeof(SpeedGraph), new PropertyMetadata(Visibility.Visible));
    public string SpeedText
    {
        get { return (string)GetValue(SpeedTextProperty); }
        set { SetValue(SpeedTextProperty, value); }
    }

    public static readonly DependencyProperty SpeedTextProperty =
        DependencyProperty.Register(nameof(SpeedText), typeof(string), typeof(SpeedGraph), new PropertyMetadata("--- MB/s"));

    public string NoDataText
    {
        get { return (string)GetValue(NoDataTextProperty); }
        set { SetValue(NoDataTextProperty, value); }
    }

    public static readonly DependencyProperty NoDataTextProperty =
        DependencyProperty.Register(nameof(NoDataText), typeof(string), typeof(SpeedGraph), new PropertyMetadata("No speed data available!"));

    public bool AutoUpdateSpeedText
    {
        get { return (bool)GetValue(AutoUpdateSpeedTextProperty); }
        set { SetValue(AutoUpdateSpeedTextProperty, value); }
    }

    public static readonly DependencyProperty AutoUpdateSpeedTextProperty =
        DependencyProperty.Register(nameof(AutoUpdateSpeedText), typeof(bool), typeof(SpeedGraph), new PropertyMetadata(true));
}
