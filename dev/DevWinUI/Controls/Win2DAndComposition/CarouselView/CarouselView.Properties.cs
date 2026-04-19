namespace DevWinUI;

public sealed partial class CarouselView
{
    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(CarouselView), new PropertyMetadata(-1, (s, e) =>{}));
    public double ItemWidth
    {
        get { return (double)GetValue(ItemWidthProperty); }
        set { SetValue(ItemWidthProperty, value); }
    }

    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(CarouselView), new PropertyMetadata(300.00d));

    public List<ICarouselViewItemSource> ItemImageSource
    {
        get { return (List<ICarouselViewItemSource>)GetValue(ItemImageSourceProperty); }
        set
        {
            SetValue(ItemImageSourceProperty, value);
            this.SetItemsImageSource();
        }
    }

    public static readonly DependencyProperty ItemImageSourceProperty =
        DependencyProperty.Register(nameof(ItemImageSource), typeof(List<ICarouselViewItemSource>), typeof(CarouselView), new PropertyMetadata(null, (s, e) =>
        {
            var carousel = s as CarouselView;
            if (carousel != null)
            {
                carousel.SetItemsImageSource();
            }
        }));
    public bool IsAutoSwitchEnabled
    {
        get { return (bool)GetValue(IsAutoSwitchEnableProperty); }
        set { SetValue(IsAutoSwitchEnableProperty, value); }
    }

    public static readonly DependencyProperty IsAutoSwitchEnableProperty =
        DependencyProperty.Register(nameof(IsAutoSwitchEnabled), typeof(bool), typeof(CarouselView), new PropertyMetadata(true, (s, e) =>
        {
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    (s as CarouselView)._dispatcherTimer.Start();
                }
                else
                {
                    (s as CarouselView)._dispatcherTimer.Stop();
                }
            }
        }));

    public TimeSpan AutoSwitchInterval
    {
        get { return (TimeSpan)GetValue(AutoSwitchIntervalProperty); }
        set { SetValue(AutoSwitchIntervalProperty, value); }
    }

    public static readonly DependencyProperty AutoSwitchIntervalProperty =
        DependencyProperty.Register(nameof(AutoSwitchInterval), typeof(TimeSpan), typeof(CarouselView), new PropertyMetadata(new TimeSpan(0, 0, 7), (s, e) =>
        {
            if (e.NewValue != e.OldValue)
            {
                (s as CarouselView)._dispatcherTimer.Interval = (TimeSpan)e.NewValue;
                (s as CarouselView)._dispatcherTimer.Start();
            }
        }));
}
