// https://github.com/DinoChan

namespace DevWinUI;
public partial class GooeyFooterEffect
{
    public static readonly DependencyProperty TimeLineFromProperty =
        DependencyProperty.Register(nameof(TimeLineFrom), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(_timeLineFromDefaultValue, OnBubbleChanged));
    public double TimeLineFrom
    {
        get { return (double)GetValue(TimeLineFromProperty); }
        set { SetValue(TimeLineFromProperty, value); }
    }

    public static readonly DependencyProperty TimeLineToProperty =
        DependencyProperty.Register(nameof(TimeLineTo), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(_timeLineToDefaultValue, OnBubbleChanged));
    public double TimeLineTo
    {
        get { return (double)GetValue(TimeLineToProperty); }
        set { SetValue(TimeLineToProperty, value); }
    }

    public static readonly DependencyProperty SizeTimeLineFromProperty =
        DependencyProperty.Register(nameof(SizeTimeLineFrom), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(_sizeTimeLineFromDefaultValue, OnBubbleChanged));
    public double SizeTimeLineFrom
    {
        get { return (double)GetValue(SizeTimeLineFromProperty); }
        set { SetValue(SizeTimeLineFromProperty, value); }
    }

    public static readonly DependencyProperty SizeTimeLineToProperty =
        DependencyProperty.Register(nameof(SizeTimeLineTo), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(0.0, OnBubbleChanged));
    public double SizeTimeLineTo
    {
        get { return (double)GetValue(SizeTimeLineToProperty); }
        set { SetValue(SizeTimeLineToProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(10.0, OnResourcePropertyValueChanged));
    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    public static readonly DependencyProperty FillProperty =
       DependencyProperty.Register(nameof(Fill), typeof(Color), typeof(GooeyFooterEffect), new PropertyMetadata(Color.FromArgb(255, 237, 85, 101), OnResourcePropertyValueChanged));
    public Color Fill
    {
        get { return (Color)GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public static readonly DependencyProperty BubbleProperty =
        DependencyProperty.Register(nameof(Bubble), typeof(int), typeof(GooeyFooterEffect), new PropertyMetadata(50, OnBubbleChanged));
    public int Bubble
    {
        get { return (int)GetValue(BubbleProperty); }
        set { SetValue(BubbleProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(_durationDefaultValue, OnBubbleChanged));
    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty SizeDurationProperty =
        DependencyProperty.Register(nameof(SizeDuration), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(_durationDefaultValue, OnBubbleChanged));
    public double SizeDuration
    {
        get { return (double)GetValue(SizeDurationProperty); }
        set { SetValue(SizeDurationProperty, value); }
    }

    public static readonly DependencyProperty BeginTimeProperty =
        DependencyProperty.Register(nameof(BeginTime), typeof(TimeSpan), typeof(GooeyFooterEffect), new PropertyMetadata(TimeSpan.FromSeconds(_sizeDurationDefaultValue), OnBubbleChanged));
    public TimeSpan BeginTime
    {
        get { return (TimeSpan)GetValue(BeginTimeProperty); }
        set { SetValue(BeginTimeProperty, value); }
    }

    public static readonly DependencyProperty SizeBeginTimeProperty =
        DependencyProperty.Register(nameof(SizeBeginTime), typeof(TimeSpan), typeof(GooeyFooterEffect), new PropertyMetadata(TimeSpan.FromSeconds(_sizeDurationDefaultValue), OnBubbleChanged));
    public TimeSpan SizeBeginTime
    {
        get { return (TimeSpan)GetValue(SizeBeginTimeProperty); }
        set { SetValue(SizeBeginTimeProperty, value); }
    }

    public static readonly DependencyProperty XProperty =
        DependencyProperty.Register(nameof(X), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(-100.0, OnPositionChanged));
    public double X
    {
        get { return (double)GetValue(XProperty); }
        set { SetValue(XProperty, value); }
    }

    public static readonly DependencyProperty YProperty =
        DependencyProperty.Register(nameof(Y), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(0.0, OnPositionChanged));
    public double Y
    {
        get { return (double)GetValue(YProperty); }
        set { SetValue(YProperty, value); }
    }

    public static readonly DependencyProperty WProperty =
        DependencyProperty.Register(nameof(W), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(200.0, OnPositionChanged));
    public double W
    {
        get { return (double)GetValue(WProperty); }
        set { SetValue(WProperty, value); }
    }

    public static readonly DependencyProperty HProperty =
        DependencyProperty.Register(nameof(H), typeof(double), typeof(GooeyFooterEffect), new PropertyMetadata(100.0, OnPositionChanged));
    public double H
    {
        get { return (double)GetValue(HProperty); }
        set { SetValue(HProperty, value); }
    }
}
