namespace DevWinUI;
public partial class Watermark
{
    public static readonly DependencyProperty MarkProperty =
        DependencyProperty.Register(nameof(Mark), typeof(string), typeof(Watermark), new PropertyMetadata(null, OnValueChanged));
    public string Mark
    {
        get { return (string)GetValue(MarkProperty); }
        set { SetValue(MarkProperty, value); }
    }

    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(Watermark), new PropertyMetadata(15.0, OnValueChanged));
    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }

    public static readonly DependencyProperty HorizontalSpacingProperty =
        DependencyProperty.Register(nameof(HorizontalSpacing), typeof(int), typeof(Watermark), new PropertyMetadata(10, OnValueChanged));
    public int HorizontalSpacing
    {
        get { return (int)GetValue(HorizontalSpacingProperty); }
        set { SetValue(HorizontalSpacingProperty, value); }
    }

    public static readonly DependencyProperty VerticalSpacingProperty =
        DependencyProperty.Register(nameof(VerticalSpacing), typeof(int), typeof(Watermark), new PropertyMetadata(5, OnValueChanged));
    public int VerticalSpacing
    {
        get { return (int)GetValue(VerticalSpacingProperty); }
        set { SetValue(VerticalSpacingProperty, value); }
    }

    public static readonly DependencyProperty CenterPointXProperty =
        DependencyProperty.Register(nameof(CenterPointX), typeof(double), typeof(Watermark), new PropertyMetadata(0d, OnValueChanged));
    public double CenterPointX
    {
        get { return (double)GetValue(CenterPointXProperty); }
        set { SetValue(CenterPointXProperty, value); }
    }

    public static readonly DependencyProperty CenterPointYProperty =
        DependencyProperty.Register(nameof(CenterPointY), typeof(double), typeof(Watermark), new PropertyMetadata(0d, OnValueChanged));
    public double CenterPointY
    {
        get { return (double)GetValue(CenterPointYProperty); }
        set { SetValue(CenterPointYProperty, value); }
    }

    public static readonly DependencyProperty WordWrappingProperty =
        DependencyProperty.Register(nameof(WordWrapping), typeof(CanvasWordWrapping), typeof(Watermark), new PropertyMetadata(CanvasWordWrapping.NoWrap, OnValueChanged));
    public CanvasWordWrapping WordWrapping
    {
        get { return (CanvasWordWrapping)GetValue(WordWrappingProperty); }
        set { SetValue(WordWrappingProperty, value); }
    }

    public static readonly DependencyProperty IsImageProperty =
        DependencyProperty.Register(nameof(IsImage), typeof(bool), typeof(Watermark), new PropertyMetadata(false, OnValueChanged));
    public bool IsImage
    {
        get { return (bool)GetValue(IsImageProperty); }
        set { SetValue(IsImageProperty, value); }
    }

    public static readonly DependencyProperty MarkImageProperty =
        DependencyProperty.Register(nameof(MarkImage), typeof(Uri), typeof(Watermark), new PropertyMetadata(null, OnValueChanged));
    public Uri MarkImage
    {
        get { return (Uri)GetValue(MarkImageProperty); }
        set { SetValue(MarkImageProperty, value); }
    }

    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(Watermark), new PropertyMetadata(Colors.Transparent, OnValueChanged));
    public Color ShadowColor
    {
        get { return (Color)GetValue(ShadowColorProperty); }
        set { SetValue(ShadowColorProperty, value); }
    }

    public static readonly DependencyProperty MarkEffectProperty =
        DependencyProperty.Register(nameof(MarkEffect), typeof(WatermarkEffect), typeof(Watermark), new PropertyMetadata(WatermarkEffect.None, OnValueChanged));
    public WatermarkEffect MarkEffect
    {
        get { return (WatermarkEffect)GetValue(MarkEffectProperty); }
        set { SetValue(MarkEffectProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(Watermark), new PropertyMetadata(4.0, OnValueChanged));
    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    public static readonly DependencyProperty MarkScaleProperty =
        DependencyProperty.Register(nameof(MarkScale), typeof(double), typeof(Watermark), new PropertyMetadata(1.0, OnValueChanged));
    public double MarkScale
    {
        get { return (double)GetValue(MarkScaleProperty); }
        set { SetValue(MarkScaleProperty, value); }
    }

    public static readonly DependencyProperty UseScaleProperty =
        DependencyProperty.Register(nameof(UseScale), typeof(bool), typeof(Watermark), new PropertyMetadata(true, OnValueChanged));
    public bool UseScale
    {
        get { return (bool)GetValue(UseScaleProperty); }
        set { SetValue(UseScaleProperty, value); }
    }
}
