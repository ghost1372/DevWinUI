namespace DevWinUI;

public partial class IndeterminateProgressBar
{
    public bool IsActive
    {
        get { return (bool)GetValue(IsActiveProperty); }
        set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(IndeterminateProgressBar), new PropertyMetadata(true, OnIsActiveChanged));

    public static readonly DependencyProperty DelayProperty =
           DependencyProperty.Register("Delay", typeof(Duration), typeof(IndeterminateProgressBar),
               new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(100)), OnDelayChanged));

    public Duration Delay
    {
        get { return (Duration)GetValue(DelayProperty); }
        set { SetValue(DelayProperty, value); }
    }

    public static readonly DependencyProperty DotWidthProperty =
        DependencyProperty.Register("DotWidth", typeof(double), typeof(IndeterminateProgressBar),
            new PropertyMetadata(4.0, OnDotWidthChanged));

    public double DotWidth
    {
        get { return (double)GetValue(DotWidthProperty); }
        set { SetValue(DotWidthProperty, value); }
    }

    public static readonly DependencyProperty DotHeightProperty =
        DependencyProperty.Register("DotHeight", typeof(double), typeof(IndeterminateProgressBar),
            new PropertyMetadata(4.0, OnDotHeightChanged));

    public double DotHeight
    {
        get { return (double)GetValue(DotHeightProperty); }
        set { SetValue(DotHeightProperty, value); }
    }

    public static readonly DependencyProperty DotRadiusXProperty =
        DependencyProperty.Register("DotRadiusX", typeof(double), typeof(IndeterminateProgressBar),
            new PropertyMetadata(0.0, OnDotRadiusXChanged));

    public double DotRadiusX
    {
        get { return (double)GetValue(DotRadiusXProperty); }
        set { SetValue(DotRadiusXProperty, value); }
    }

    public static readonly DependencyProperty DotRadiusYProperty =
        DependencyProperty.Register("DotRadiusY", typeof(double), typeof(IndeterminateProgressBar),
            new PropertyMetadata(0.0, OnDotRadiusYChanged));

    public double DotRadiusY
    {
        get { return (double)GetValue(DotRadiusYProperty); }
        set { SetValue(DotRadiusYProperty, value); }
    }

    public static readonly DependencyProperty DurationAProperty =
        DependencyProperty.Register("DurationA", typeof(Duration), typeof(IndeterminateProgressBar),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.5)), OnDurationAChanged));

    public Duration DurationA
    {
        get { return (Duration)GetValue(DurationAProperty); }
        set { SetValue(DurationAProperty, value); }
    }

    public static readonly DependencyProperty DurationBProperty =
        DependencyProperty.Register("DurationB", typeof(Duration), typeof(IndeterminateProgressBar),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(1.5)), OnDurationBChanged));

    public Duration DurationB
    {
        get { return (Duration)GetValue(DurationBProperty); }
        set { SetValue(DurationBProperty, value); }
    }

    public static readonly DependencyProperty DurationCProperty =
        DependencyProperty.Register("DurationC", typeof(Duration), typeof(IndeterminateProgressBar),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.5)), OnDurationCChanged));

    public Duration DurationC
    {
        get { return (Duration)GetValue(DurationCProperty); }
        set { SetValue(DurationCProperty, value); }
    }

    public static readonly DependencyProperty KeyFrameAProperty =
        DependencyProperty.Register("KeyFrameA", typeof(double), typeof(IndeterminateProgressBar),
            new PropertyMetadata(0.33, OnKeyFrameAChanged));

    public double KeyFrameA
    {
        get { return (double)GetValue(KeyFrameAProperty); }
        set { SetValue(KeyFrameAProperty, value); }
    }

    public static readonly DependencyProperty KeyFrameBProperty =
        DependencyProperty.Register("KeyFrameB", typeof(double), typeof(IndeterminateProgressBar),
            new PropertyMetadata(0.63, OnKeyFrameBChanged));

    public double KeyFrameB
    {
        get { return (double)GetValue(KeyFrameBProperty); }
        set { SetValue(KeyFrameBProperty, value); }
    }

    public static readonly DependencyProperty OscillateProperty =
        DependencyProperty.Register("Oscillate", typeof(bool), typeof(IndeterminateProgressBar),
            new PropertyMetadata(false, OnOscillateChanged));

    public bool Oscillate
    {
        get { return (bool)GetValue(OscillateProperty); }
        set { SetValue(OscillateProperty, value); }
    }

    public static readonly DependencyProperty ReverseDurationProperty =
       DependencyProperty.Register("ReverseDuration", typeof(Duration), typeof(IndeterminateProgressBar),
           new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2.9)), OnReverseDurationChanged));

    public Duration ReverseDuration
    {
        get { return (Duration)GetValue(ReverseDurationProperty); }
        set { SetValue(ReverseDurationProperty, value); }
    }

    public static readonly DependencyProperty TotalDurationProperty =
        DependencyProperty.Register("TotalDuration", typeof(Duration), typeof(IndeterminateProgressBar),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(4.4)), OnTotalDurationChanged));

    public Duration TotalDuration
    {
        get { return (Duration)GetValue(TotalDurationProperty); }
        set { SetValue(TotalDurationProperty, value); }
    }
}
