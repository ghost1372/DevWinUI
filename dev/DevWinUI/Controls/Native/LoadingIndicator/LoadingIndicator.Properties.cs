namespace DevWinUI;

public partial class LoadingIndicator
{
    public LoadingIndicatorMode Mode
    {
        get { return (LoadingIndicatorMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(LoadingIndicatorMode), typeof(LoadingIndicator), new PropertyMetadata(default(LoadingIndicatorMode), OnModeChanged));

    public double SpeedRatio
    {
        get { return (double)GetValue(SpeedRatioProperty); }
        set { SetValue(SpeedRatioProperty, value); }
    }

    public static readonly DependencyProperty SpeedRatioProperty =
        DependencyProperty.Register(nameof(SpeedRatio), typeof(double), typeof(LoadingIndicator), new PropertyMetadata(1d, OnSpeedRatioChanged));

    public bool IsActive
    {
        get { return (bool)GetValue(IsActiveProperty); }
        set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(LoadingIndicator), new PropertyMetadata(true, OnIsActiveChanged));

    public double IndicatorSize
    {
        get { return (double)GetValue(IndicatorSizeProperty); }
        set { SetValue(IndicatorSizeProperty, value); }
    }

    public static readonly DependencyProperty IndicatorSizeProperty =
        DependencyProperty.Register(nameof(IndicatorSize), typeof(double), typeof(LoadingIndicator), new PropertyMetadata(double.NaN, OnRebuild));

    public double AnimationDuration
    {
        get { return (double)GetValue(AnimationDurationProperty); }
        set { SetValue(AnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty AnimationDurationProperty =
        DependencyProperty.Register(nameof(AnimationDuration), typeof(double), typeof(LoadingIndicator), new PropertyMetadata(double.NaN, OnDurationChanged));

    public double ItemSize
    {
        get { return (double)GetValue(ItemSizeProperty); }
        set { SetValue(ItemSizeProperty, value); }
    }

    public static readonly DependencyProperty ItemSizeProperty =
        DependencyProperty.Register(nameof(ItemSize), typeof(double), typeof(LoadingIndicator), new PropertyMetadata(15d, OnRebuild));

    public int ItemCount
    {
        get { return (int)GetValue(ItemCountProperty); }
        set { SetValue(ItemCountProperty, value); }
    }

    public static readonly DependencyProperty ItemCountProperty =
        DependencyProperty.Register(nameof(ItemCount), typeof(int), typeof(LoadingIndicator), new PropertyMetadata(3, OnRebuild));

    public double Phase
    {
        get { return (double)GetValue(PhaseProperty); }
        set { SetValue(PhaseProperty, value); }
    }

    public static readonly DependencyProperty PhaseProperty =
        DependencyProperty.Register(nameof(Phase), typeof(double), typeof(LoadingIndicator), new PropertyMetadata(0d, OnPhaseChanged));
}
