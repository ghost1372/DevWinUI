namespace DevWinUI;

public partial class StoreCarousel
{
    public bool UseImageEdgeOverContentColor
    {
        get { return (bool)GetValue(UseImageEdgeOverContentColorProperty); }
        set { SetValue(UseImageEdgeOverContentColorProperty, value); }
    }

    public static readonly DependencyProperty UseImageEdgeOverContentColorProperty =
        DependencyProperty.Register(nameof(UseImageEdgeOverContentColor), typeof(bool), typeof(StoreCarousel), new PropertyMetadata(false));

    public TimeSpan ShuffleDuration
    {
        get { return (TimeSpan)GetValue(ShuffleDurationProperty); }
        set { SetValue(ShuffleDurationProperty, value); }
    }

    public static readonly DependencyProperty ShuffleDurationProperty =
        DependencyProperty.Register(nameof(ShuffleDuration), typeof(TimeSpan), typeof(StoreCarousel), new PropertyMetadata(TimeSpan.FromSeconds(5), OnTimerIntervalChanged));
    private static void OnTimerIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StoreCarousel)d;
        if (ctl != null && ctl.timer != null)
        {
            ctl.timer.Interval = (TimeSpan)e.NewValue;
        }
    }

    public bool AutoShuffle
    {
        get => (bool)GetValue(AutoShuffleProperty);
        set => SetValue(AutoShuffleProperty, value);
    }

    public static readonly DependencyProperty AutoShuffleProperty =
        DependencyProperty.Register(nameof(AutoShuffle), typeof(bool), typeof(StoreCarousel), new PropertyMetadata(true, OnAutoShuffleChanged));

    private static void OnAutoShuffleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StoreCarousel)d;
        if (ctl != null && ctl.timer != null)
        {
            if ((bool)e.NewValue)
                ctl.timer.Start();
            else
                ctl.timer.Stop();
        }
    }

    public Visibility PipsPagerVisibility
    {
        get { return (Visibility)GetValue(PipsPagerVisibilityProperty); }
        set { SetValue(PipsPagerVisibilityProperty, value); }
    }

    public static readonly DependencyProperty PipsPagerVisibilityProperty =
        DependencyProperty.Register(nameof(PipsPagerVisibility), typeof(Visibility), typeof(StoreCarousel), new PropertyMetadata(Visibility.Visible));

    public object PrimaryImageSource
    {
        get { return (object)GetValue(PrimaryImageSourceProperty); }
        set { SetValue(PrimaryImageSourceProperty, value); }
    }

    public static readonly DependencyProperty PrimaryImageSourceProperty =
        DependencyProperty.Register(nameof(PrimaryImageSource), typeof(object), typeof(StoreCarousel), new PropertyMetadata(null));

    public object SecondaryImageSource
    {
        get { return (object)GetValue(SecondaryImageSourceProperty); }
        set { SetValue(SecondaryImageSourceProperty, value); }
    }

    public static readonly DependencyProperty SecondaryImageSourceProperty =
        DependencyProperty.Register(nameof(SecondaryImageSource), typeof(object), typeof(StoreCarousel), new PropertyMetadata(null));

    public object TertiaryImageSource
    {
        get { return (object)GetValue(TertiaryImageSourceProperty); }
        set { SetValue(TertiaryImageSourceProperty, value); }
    }

    public static readonly DependencyProperty TertiaryImageSourceProperty =
        DependencyProperty.Register(nameof(TertiaryImageSource), typeof(object), typeof(StoreCarousel), new PropertyMetadata(null));

    public IList<StoreCarouselItem> ItemsSource
    {
        get { return (IList<StoreCarouselItem>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IList<StoreCarouselItem>), typeof(StoreCarousel), new PropertyMetadata(null, OnItemsSourceChanged));
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (StoreCarousel)d;
        control.OnItemsSourceChanged((IList<StoreCarouselItem>)e.NewValue);
    }
}
