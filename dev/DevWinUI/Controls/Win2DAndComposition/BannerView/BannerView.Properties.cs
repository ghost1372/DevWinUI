namespace DevWinUI;

public partial class BannerView
{
    public bool IsPerspectiveEnable
    {
        get { return (bool)GetValue(IsPerspectiveEnableProperty); }
        set { SetValue(IsPerspectiveEnableProperty, value); }
    }
    public static readonly DependencyProperty IsPerspectiveEnableProperty =
        DependencyProperty.Register(nameof(IsPerspectiveEnable), typeof(bool), typeof(BannerView), new PropertyMetadata(false, (s, a) => ((BannerView)s).UpdatePerspective()));

    public bool IsScaleEnable
    {
        get { return (bool)GetValue(IsScaleEnableProperty); }
        set { SetValue(IsScaleEnableProperty, value); }
    }
    public static readonly DependencyProperty IsScaleEnableProperty =
        DependencyProperty.Register(nameof(IsScaleEnable), typeof(bool), typeof(BannerView), new PropertyMetadata(false, (s, a) => ((BannerView)s).UpdateScale()));

    public double ItemsSpacing
    {
        get { return (double)GetValue(ItemsSpacingProperty); }
        set { SetValue(ItemsSpacingProperty, value); }
    }

    public static readonly DependencyProperty ItemsSpacingProperty =
        DependencyProperty.Register(nameof(ItemsSpacing), typeof(double), typeof(BannerView), new PropertyMetadata(0d, (s, a) => ((BannerView)s).UpdateItemSpacing()));

    public bool AutoShuffle
    {
        get { return (bool)GetValue(AutoShuffleProperty); }
        set { SetValue(AutoShuffleProperty, value); }
    }

    public static readonly DependencyProperty AutoShuffleProperty =
        DependencyProperty.Register(nameof(AutoShuffle), typeof(bool), typeof(BannerView), new PropertyMetadata(false, OnAutoShuffleChanged));

    private static void OnAutoShuffleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BannerView)d;
        ctl.OnAutoShuffleChanged();
    }

    public TimeSpan Interval
    {
        get { return (TimeSpan)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register(nameof(Interval), typeof(TimeSpan), typeof(BannerView), new PropertyMetadata(TimeSpan.FromSeconds(1), OnIntervalChanged));

    private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BannerView)d;
        ctl.timer.Interval = (TimeSpan)e.NewValue;
    }

    public CarouselShiftingDirection ShiftingDirection
    {
        get { return (CarouselShiftingDirection)GetValue(ShiftingDirectionProperty); }
        set { SetValue(ShiftingDirectionProperty, value); }
    }

    public static readonly DependencyProperty ShiftingDirectionProperty =
        DependencyProperty.Register(nameof(ShiftingDirection), typeof(CarouselShiftingDirection), typeof(BannerView), new PropertyMetadata(CarouselShiftingDirection.Forward, null));
}
