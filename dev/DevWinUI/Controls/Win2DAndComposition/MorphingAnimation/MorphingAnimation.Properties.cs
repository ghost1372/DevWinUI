namespace DevWinUI;

public partial class MorphingAnimation
{
    public Color PrimaryColor
    {
        get { return (Color)GetValue(PrimaryColorProperty); }
        set { SetValue(PrimaryColorProperty, value); }
    }

    public static readonly DependencyProperty PrimaryColorProperty =
        DependencyProperty.Register(nameof(PrimaryColor), typeof(Color), typeof(MorphingAnimation), new PropertyMetadata(Colors.Transparent, OnPrimaryColorChanged));
    private static void OnPrimaryColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.primaryColor = (Color)e.NewValue;
    }

    public Color SecondaryColor
    {
        get { return (Color)GetValue(SecondaryColorProperty); }
        set { SetValue(SecondaryColorProperty, value); }
    }

    public static readonly DependencyProperty SecondaryColorProperty =
        DependencyProperty.Register(nameof(SecondaryColor), typeof(Color), typeof(MorphingAnimation), new PropertyMetadata(Colors.Transparent, OnSecondaryColorChanged));


    private static void OnSecondaryColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.secondaryColor = (Color)e.NewValue;
    }

    public TimeSpan AnimationDuration
    {
        get { return (TimeSpan)GetValue(AnimationDurationProperty); }
        set { SetValue(AnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty AnimationDurationProperty =
        DependencyProperty.Register(nameof(AnimationDuration), typeof(TimeSpan), typeof(MorphingAnimation), new PropertyMetadata(TimeSpan.FromMilliseconds(500), OnDurationChanged));

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.animDuration = ((TimeSpan)e.NewValue).TotalSeconds;
    }

    public TimeSpan HoldDuration
    {
        get { return (TimeSpan)GetValue(HoldDurationProperty); }
        set { SetValue(HoldDurationProperty, value); }
    }

    public static readonly DependencyProperty HoldDurationProperty =
        DependencyProperty.Register(nameof(HoldDuration), typeof(TimeSpan), typeof(MorphingAnimation), new PropertyMetadata(TimeSpan.FromSeconds(1), OnHoldDurationChanged));

    private static void OnHoldDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.holdDuration = ((TimeSpan)e.NewValue).TotalSeconds;
    }

    public string PrimaryData
    {
        get { return (string)GetValue(PrimaryDataProperty); }
        set { SetValue(PrimaryDataProperty, value); }
    }

    public static readonly DependencyProperty PrimaryDataProperty =
        DependencyProperty.Register(nameof(PrimaryData), typeof(string), typeof(MorphingAnimation), new PropertyMetadata(null, OnPrimaryDataChanged));

    private static void OnPrimaryDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.primaryData = (string)e.NewValue;
        ctl.GetShapesPoint();
        ctl.canvas?.Invalidate();
    }

    public string SecondaryData
    {
        get { return (string)GetValue(SecondaryDataProperty); }
        set { SetValue(SecondaryDataProperty, value); }
    }

    public static readonly DependencyProperty SecondaryDataProperty =
        DependencyProperty.Register(nameof(SecondaryData), typeof(string), typeof(MorphingAnimation), new PropertyMetadata(null, OnSecondaryDataChanged));

    private static void OnSecondaryDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.secondaryData = (string)e.NewValue;
        ctl.GetShapesPoint();
        ctl.canvas?.Invalidate();
    }

    public int PointCount
    {
        get { return (int)GetValue(PointCountProperty); }
        set { SetValue(PointCountProperty, value); }
    }

    public static readonly DependencyProperty PointCountProperty =
        DependencyProperty.Register(nameof(PointCount), typeof(int), typeof(MorphingAnimation), new PropertyMetadata(300, OnPointCountChanged));

    private static void OnPointCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MorphingAnimation)d;
        ctl.pointCount = (int)e.NewValue;
        ctl.canvas?.Invalidate();
    }
}
