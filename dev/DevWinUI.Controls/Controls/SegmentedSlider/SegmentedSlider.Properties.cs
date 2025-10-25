namespace DevWinUI;

public partial class SegmentedSlider
{
    public HorizontalAlignment TitleHorizontalAlignment
    {
        get { return (HorizontalAlignment)GetValue(TitleHorizontalAlignmentProperty); }
        set { SetValue(TitleHorizontalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty TitleHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(TitleHorizontalAlignment), typeof(HorizontalAlignment), typeof(SegmentedSlider), new PropertyMetadata(HorizontalAlignment.Center, OnTitleHorizontalAlignmentChanged));

    private static void OnTitleHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SegmentedSlider)d;
        if (ctl != null)
        {
            ctl.UpdateTitleHorizontalAlignment();
        }
    }

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(SegmentedSlider), new PropertyMetadata(5.0));

    public TimeSpan SelectedTime
    {
        get => (TimeSpan)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register(nameof(SelectedTime), typeof(TimeSpan), typeof(SegmentedSlider), new PropertyMetadata(TimeSpan.Zero, OnSelectedTimeChanged));

    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SegmentedSlider)d;
        if (ctl != null)
        {
            ctl.OnSelectedTimeChanged((TimeSpan)e.NewValue);
        }
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(SegmentedSlider), new PropertyMetadata(0.0, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SegmentedSlider)d;
        if (ctl != null)
        {
            ctl.OnValueChanged();
        }
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(SegmentedSlider), new PropertyMetadata(100.0));

    public int SegmentCount
    {
        get => (int)GetValue(SegmentCountProperty);
        set => SetValue(SegmentCountProperty, value);
    }

    public static readonly DependencyProperty SegmentCountProperty =
        DependencyProperty.Register(nameof(SegmentCount), typeof(int), typeof(SegmentedSlider), new PropertyMetadata(5, OnSegmentCountChanged));

    private static void OnSegmentCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SegmentedSlider slider)
            slider.UpdateSegments();
    }

    public SegmentedSliderTitleVisibility TitleVisibility
    {
        get => (SegmentedSliderTitleVisibility)GetValue(TitleVisibilityProperty);
        set => SetValue(TitleVisibilityProperty, value);
    }

    public static readonly DependencyProperty TitleVisibilityProperty =
        DependencyProperty.Register(nameof(TitleVisibility), typeof(SegmentedSliderTitleVisibility), typeof(SegmentedSlider), new PropertyMetadata(SegmentedSliderTitleVisibility.AlwaysVisible, OnTitleVisibilityChanged));

    private static void OnTitleVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SegmentedSlider slider)
        {
            slider.UpdateSegmentLabelsVisibility();
        }
    }

    public IList<string> SegmentTitles
    {
        get => (IList<string>)GetValue(SegmentTitlesProperty);
        set => SetValue(SegmentTitlesProperty, value);
    }

    public static readonly DependencyProperty SegmentTitlesProperty =
        DependencyProperty.Register(nameof(SegmentTitles), typeof(IList<string>), typeof(SegmentedSlider), new PropertyMetadata(null, OnSegmentTitlesChanged));

    private static void OnSegmentTitlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SegmentedSlider slider)
        {
            slider.UpdateSegments();
        }
    }

    public TimeSpan TotalTime
    {
        get => (TimeSpan)GetValue(TotalTimeProperty);
        set => SetValue(TotalTimeProperty, value);
    }

    public static readonly DependencyProperty TotalTimeProperty =
        DependencyProperty.Register(nameof(TotalTime), typeof(TimeSpan), typeof(SegmentedSlider), new PropertyMetadata(TimeSpan.Zero, OnTotalTimeChanged));

    private static void OnTotalTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SegmentedSlider slider)
            slider.UpdateSegments();
    }

    public IList<SegmentedSliderTimeInfo> TimeSegments
    {
        get => (IList<SegmentedSliderTimeInfo>)GetValue(TimeSegmentsProperty);
        set => SetValue(TimeSegmentsProperty, value);
    }

    public static readonly DependencyProperty TimeSegmentsProperty =
        DependencyProperty.Register(nameof(TimeSegments), typeof(IList<SegmentedSliderTimeInfo>), typeof(SegmentedSlider), new PropertyMetadata(null, OnTimeSegmentsChanged));

    private static void OnTimeSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SegmentedSlider slider)
        {
            slider.NormalizeSegments();
            slider.UpdateSegments();
        }
    }
}
