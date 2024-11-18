namespace DevWinUI;
public partial class GooeyEffect
{
    public static readonly DependencyProperty SecondaryTimeLineFromXProperty =
        DependencyProperty.Register(nameof(SecondaryTimeLineFromX), typeof(double), typeof(GooeyEffect), new PropertyMetadata(-100.0, OnAnimationChanged));
    public double SecondaryTimeLineFromX
    {
        get { return (double)GetValue(SecondaryTimeLineFromXProperty); }
        set { SetValue(SecondaryTimeLineFromXProperty, value); }
    }

    public static readonly DependencyProperty SecondaryTimeLineToXProperty =
        DependencyProperty.Register(nameof(SecondaryTimeLineToX), typeof(double), typeof(GooeyEffect), new PropertyMetadata(100.0, OnAnimationChanged));
    public double SecondaryTimeLineToX
    {
        get { return (double)GetValue(SecondaryTimeLineToXProperty); }
        set { SetValue(SecondaryTimeLineToXProperty, value); }
    }

    public static readonly DependencyProperty PrimaryTimeLineFromXProperty =
        DependencyProperty.Register(nameof(PrimaryTimeLineFromX), typeof(double), typeof(GooeyEffect), new PropertyMetadata(100.0, OnAnimationChanged));
    public double PrimaryTimeLineFromX
    {
        get { return (double)GetValue(PrimaryTimeLineFromXProperty); }
        set { SetValue(PrimaryTimeLineFromXProperty, value); }
    }

    public static readonly DependencyProperty PrimaryTimeLineToXProperty =
        DependencyProperty.Register(nameof(PrimaryTimeLineToX), typeof(double), typeof(GooeyEffect), new PropertyMetadata(-100.0, OnAnimationChanged));
    public double PrimaryTimeLineToX
    {
        get { return (double)GetValue(PrimaryTimeLineToXProperty); }
        set { SetValue(PrimaryTimeLineToXProperty, value); }
    }

    public static readonly DependencyProperty SecondaryTimeLineFromYProperty =
        DependencyProperty.Register(nameof(SecondaryTimeLineFromY), typeof(double), typeof(GooeyEffect), new PropertyMetadata(0.0, OnAnimationChanged));
    public double SecondaryTimeLineFromY
    {
        get { return (double)GetValue(SecondaryTimeLineFromYProperty); }
        set { SetValue(SecondaryTimeLineFromYProperty, value); }
    }

    public static readonly DependencyProperty SecondaryTimeLineToYProperty =
        DependencyProperty.Register(nameof(SecondaryTimeLineToY), typeof(double), typeof(GooeyEffect), new PropertyMetadata(0.0, OnAnimationChanged));
    public double SecondaryTimeLineToY
    {
        get { return (double)GetValue(SecondaryTimeLineToYProperty); }
        set { SetValue(SecondaryTimeLineToYProperty, value); }
    }

    public static readonly DependencyProperty PrimaryTimeLineFromYProperty =
        DependencyProperty.Register(nameof(PrimaryTimeLineFromY), typeof(double), typeof(GooeyEffect), new PropertyMetadata(0.0, OnAnimationChanged));
    public double PrimaryTimeLineFromY
    {
        get { return (double)GetValue(PrimaryTimeLineFromYProperty); }
        set { SetValue(PrimaryTimeLineFromYProperty, value); }
    }

    public static readonly DependencyProperty PrimaryTimeLineToYProperty =
        DependencyProperty.Register(nameof(PrimaryTimeLineToY), typeof(double), typeof(GooeyEffect), new PropertyMetadata(0.0, OnAnimationChanged));
    public double PrimaryTimeLineToY
    {
        get { return (double)GetValue(PrimaryTimeLineToYProperty); }
        set { SetValue(PrimaryTimeLineToYProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(GooeyEffect), new PropertyMetadata(2.0, OnAnimationChanged));
    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty EasingProperty =
        DependencyProperty.Register(nameof(Easing), typeof(EasingMode), typeof(GooeyEffect), new PropertyMetadata(EasingMode.EaseInOut, OnAnimationChanged));
    public EasingMode Easing
    {
        get { return (EasingMode)GetValue(EasingProperty); }
        set { SetValue(EasingProperty, value); }
    }

    public static readonly DependencyProperty SecondaryFillProperty =
        DependencyProperty.Register(nameof(SecondaryFill), typeof(Color), typeof(GooeyEffect), new PropertyMetadata(Colors.IndianRed, OnResourcePropertyValueChanged));
    public Color SecondaryFill
    {
        get { return (Color)GetValue(SecondaryFillProperty); }
        set { SetValue(SecondaryFillProperty, value); }
    }

    public static readonly DependencyProperty PrimaryFillProperty =
        DependencyProperty.Register(nameof(PrimaryFill), typeof(Color), typeof(GooeyEffect), new PropertyMetadata(Colors.PaleVioletRed, OnResourcePropertyValueChanged));
    public Color PrimaryFill
    {
        get { return (Color)GetValue(PrimaryFillProperty); }
        set { SetValue(PrimaryFillProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(GooeyEffect), new PropertyMetadata(20.0, OnResourcePropertyValueChanged));
    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    public static readonly DependencyProperty SecondaryRadiusProperty =
        DependencyProperty.Register(nameof(SecondaryRadius), typeof(double), typeof(GooeyEffect), new PropertyMetadata(30.0, OnPropertyValueChanged));
    public double SecondaryRadius
    {
        get { return (double)GetValue(SecondaryRadiusProperty); }
        set { SetValue(SecondaryRadiusProperty, value); }
    }

    public static readonly DependencyProperty PrimaryRadiusProperty =
        DependencyProperty.Register(nameof(PrimaryRadius), typeof(double), typeof(GooeyEffect), new PropertyMetadata(30.0, OnPropertyValueChanged));

    public double PrimaryRadius
    {
        get { return (double)GetValue(PrimaryRadiusProperty); }
        set { SetValue(PrimaryRadiusProperty, value); }
    }
}
