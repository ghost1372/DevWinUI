namespace DevWinUI;
public partial class StepBar
{
    public StepStatus Status
    {
        get => (StepStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(StepStatus), typeof(StepBar), new PropertyMetadata(StepStatus.Info, OnStepStatusChanged));

    private static void OnStepStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBar)d;
        if (ctl != null)
        {
            ctl.UpdateItems();
        }
    }

    public int StepIndex
    {
        get => (int)GetValue(StepIndexProperty);
        set
        {
            if (ItemsCount > 0)
            {
                int clampedValue = Math.Max(0, Math.Min(ItemsCount - 1, value));
                SetValue(StepIndexProperty, clampedValue);
            }
            else
            {
                SetValue(StepIndexProperty, value);
            }
        }
    }

    public static readonly DependencyProperty StepIndexProperty =
        DependencyProperty.Register(nameof(StepIndex), typeof(int), typeof(StepBar), new PropertyMetadata(0, OnStepIndexChanged));

    private static void OnStepIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBar)d;
        if (ctl != null)
        {
            ctl.OnStepIndexChanged((int)e.NewValue);
        }
    }

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(StepBar), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBar)d;
        if (ctl != null)
        {
            ctl.UpdateItemsPanel();
            ctl.UpdateProgressBarSize();
            ctl.OnStepIndexChanged(ctl.StepIndex);
            ctl.UpdateItems();
        }
    }

    public StepBarHeaderDisplayMode HeaderDisplayMode
    {
        get { return (StepBarHeaderDisplayMode)GetValue(HeaderDisplayModeProperty); }
        set { SetValue(HeaderDisplayModeProperty, value); }
    }

    public static readonly DependencyProperty HeaderDisplayModeProperty =
        DependencyProperty.Register(nameof(HeaderDisplayMode), typeof(StepBarHeaderDisplayMode), typeof(StepBar), new PropertyMetadata(StepBarHeaderDisplayMode.Bottom, OnHeaderDisplayModeChanged));

    private static void OnHeaderDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBar)d;
        if (ctl != null)
        {
            ctl.UpdateHeaderDisplayMode();
        }
    }

    public bool ShowStepIndex
    {
        get { return (bool)GetValue(ShowStepIndexProperty); }
        set { SetValue(ShowStepIndexProperty, value); }
    }

    public static readonly DependencyProperty ShowStepIndexProperty =
        DependencyProperty.Register(nameof(ShowStepIndex), typeof(bool), typeof(StepBar), new PropertyMetadata(true, OnShowStepIndexChanged));

    private static void OnShowStepIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBar)d;
        if (ctl != null)
        {
            ctl.UpdateItems();
        }
    }

    public IconSource UnderWayIcon
    {
        get { return (IconSource)GetValue(UnderWayIconProperty); }
        set { SetValue(UnderWayIconProperty, value); }
    }

    public static readonly DependencyProperty UnderWayIconProperty =
        DependencyProperty.Register(nameof(UnderWayIcon), typeof(IconSource), typeof(StepBar), new PropertyMetadata(null));

    public IconSource WaitingIcon
    {
        get { return (IconSource)GetValue(WaitingIconProperty); }
        set { SetValue(WaitingIconProperty, value); }
    }

    public static readonly DependencyProperty WaitingIconProperty =
        DependencyProperty.Register(nameof(WaitingIcon), typeof(IconSource), typeof(StepBar), new PropertyMetadata(null, OnIconChanged));

    public IconSource CompleteIcon
    {
        get { return (IconSource)GetValue(CompleteIconProperty); }
        set { SetValue(CompleteIconProperty, value); }
    }

    public static readonly DependencyProperty CompleteIconProperty =
        DependencyProperty.Register(nameof(CompleteIcon), typeof(IconSource), typeof(StepBar), new PropertyMetadata(null, OnIconChanged));

    public StepBarDisplayMode DisplayMode
    {
        get { return (StepBarDisplayMode)GetValue(DisplayModeProperty); }
        set { SetValue(DisplayModeProperty, value); }
    }

    public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(StepBarDisplayMode), typeof(StepBar), new PropertyMetadata(StepBarDisplayMode.Index, OnIconChanged));

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBar)d;
        if (ctl != null)
        {
            ctl.UpdateItems();
        }
    }

    public bool AutoSelectOnItemClick
    {
        get => (bool)GetValue(AutoSelectOnItemClickProperty);
        set => SetValue(AutoSelectOnItemClickProperty, value);
    }

    public static readonly DependencyProperty AutoSelectOnItemClickProperty =
        DependencyProperty.Register(nameof(AutoSelectOnItemClick), typeof(bool), typeof(StepBar), new PropertyMetadata(false));
}
