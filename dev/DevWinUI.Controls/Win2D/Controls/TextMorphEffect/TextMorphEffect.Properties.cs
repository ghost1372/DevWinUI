// https://github.com/DinoChan

using Microsoft.UI.Text;
using Windows.UI.Text;

namespace DevWinUI;
public partial class TextMorphEffect
{
    public static readonly DependencyProperty EasingProperty =
        DependencyProperty.Register(nameof(Easing), typeof(EasingMode), typeof(TextMorphEffect), new PropertyMetadata(EasingMode.EaseOut, OnAnimationChanged));
    public EasingMode Easing
    {
        get { return (EasingMode)GetValue(EasingProperty); }
        set { SetValue(EasingProperty, value); }
    }

    public static readonly DependencyProperty TimeLineFromProperty =
        DependencyProperty.Register(nameof(TimeLineFrom), typeof(double), typeof(TextMorphEffect), new PropertyMetadata(0.0, OnAnimationChanged));
    public double TimeLineFrom
    {
        get { return (double)GetValue(TimeLineFromProperty); }
        set { SetValue(TimeLineFromProperty, value); }
    }

    public static readonly DependencyProperty TimeLineToProperty =
        DependencyProperty.Register(nameof(TimeLineTo), typeof(double), typeof(TextMorphEffect), new PropertyMetadata(1.0, OnAnimationChanged));
    public double TimeLineTo
    {
        get { return (double)GetValue(TimeLineToProperty); }
        set { SetValue(TimeLineToProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(TextMorphEffect), new PropertyMetadata(1.0, OnAnimationChanged));
    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty BeginTimeProperty =
        DependencyProperty.Register(nameof(BeginTime), typeof(TimeSpan), typeof(TextMorphEffect), new PropertyMetadata(TimeSpan.FromSeconds(2)));
    public TimeSpan BeginTime
    {
        get { return (TimeSpan)GetValue(BeginTimeProperty); }
        set { SetValue(BeginTimeProperty, value); }
    }

    public new static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(TextMorphEffect), new PropertyMetadata(100.0, OnAnimationChanged));
    public new double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public new static readonly DependencyProperty FontWeightProperty =
        DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(TextMorphEffect), new PropertyMetadata(FontWeights.Bold, OnAnimationChanged));
    public new FontWeight FontWeight
    {
        get { return (FontWeight)GetValue(FontWeightProperty); }
        set { SetValue(FontWeightProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextMorphEffect), new PropertyMetadata(null, OnTextChanged));
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty DelimiterProperty =
        DependencyProperty.Register(nameof(Delimiter), typeof(string), typeof(TextMorphEffect), new PropertyMetadata(",", OnTextChanged));
    public string Delimiter
    {
        get { return (string)GetValue(DelimiterProperty); }
        set { SetValue(DelimiterProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(CanvasTextDirection), typeof(TextMorphEffect), new PropertyMetadata(CanvasTextDirection.LeftToRightThenTopToBottom, OnAnimationChanged));
    public CanvasTextDirection Direction
    {
        get { return (CanvasTextDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public new static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.Register(nameof(VerticalAlignment), typeof(CanvasVerticalAlignment), typeof(TextMorphEffect), new PropertyMetadata(CanvasVerticalAlignment.Center, OnAnimationChanged));
    public new CanvasVerticalAlignment VerticalAlignment
    {
        get { return (CanvasVerticalAlignment)GetValue(VerticalAlignmentProperty); }
        set { SetValue(VerticalAlignmentProperty, value); }
    }

    public new static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(HorizontalAlignment), typeof(CanvasHorizontalAlignment), typeof(TextMorphEffect), new PropertyMetadata(CanvasHorizontalAlignment.Center, OnAnimationChanged));
    public new CanvasHorizontalAlignment HorizontalAlignment
    {
        get { return (CanvasHorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
        set { SetValue(HorizontalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty AutoReverseProperty =
        DependencyProperty.Register(nameof(AutoReverse), typeof(bool), typeof(TextMorphEffect), new PropertyMetadata(true, OnAnimationChanged));
    public bool AutoReverse
    {
        get { return (bool)GetValue(AutoReverseProperty); }
        set { SetValue(AutoReverseProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(TextMorphEffect), new PropertyMetadata(0.0, OnResourcePropertyValueChanged));
    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }
    public static readonly DependencyProperty ColorBrushProperty =
       DependencyProperty.Register(nameof(ColorBrush), typeof(Color), typeof(TextMorphEffect), new PropertyMetadata(null, OnPropertyChanged));
    public Color ColorBrush
    {
        get { return (Color)GetValue(ColorBrushProperty); }
        set { SetValue(ColorBrushProperty, value); }
    }

    public static readonly DependencyProperty MorphSpeedProperty =
        DependencyProperty.Register(nameof(MorphSpeed), typeof(int), typeof(TextMorphEffect), new PropertyMetadata(2000, OnPropertyChanged));
    public int MorphSpeed
    {
        get { return (int)GetValue(MorphSpeedProperty); }
        set { SetValue(MorphSpeedProperty, value); }
    }
}
