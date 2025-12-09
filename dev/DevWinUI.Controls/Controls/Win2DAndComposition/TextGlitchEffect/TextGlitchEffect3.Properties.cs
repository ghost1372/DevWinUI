// https://github.com/DinoChan

namespace DevWinUI;
public partial class TextGlitchEffect3
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextGlitchEffect3), new PropertyMetadata(null, OnValueChanged));
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty DelimiterProperty =
        DependencyProperty.Register(nameof(Delimiter), typeof(string), typeof(TextGlitchEffect3), new PropertyMetadata(",", OnValueChanged));
    public string Delimiter
    {
        get { return (string)GetValue(DelimiterProperty); }
        set { SetValue(DelimiterProperty, value); }
    }

    public static readonly DependencyProperty DelayProperty =
        DependencyProperty.Register(nameof(Delay), typeof(TimeSpan), typeof(TextGlitchEffect3), new PropertyMetadata(TimeSpan.FromSeconds(1), OnValueChanged));
    public TimeSpan Delay
    {
        get { return (TimeSpan)GetValue(DelayProperty); }
        set { SetValue(DelayProperty, value); }
    }

    public static readonly DependencyProperty StoryBoardAnimationDurationProperty =
        DependencyProperty.Register(nameof(StoryBoardAnimationDuration), typeof(TimeSpan), typeof(TextGlitchEffect3), new PropertyMetadata(TimeSpan.FromSeconds(1.5), OnValueChanged));
    public TimeSpan StoryBoardAnimationDuration
    {
        get { return (TimeSpan)GetValue(StoryBoardAnimationDurationProperty); }
        set { SetValue(StoryBoardAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty OffsetAnimationDurationProperty =
        DependencyProperty.Register(nameof(OffsetAnimationDuration), typeof(TimeSpan), typeof(TextGlitchEffect3), new PropertyMetadata(TimeSpan.FromSeconds(3), OnValueChanged));
    public TimeSpan OffsetAnimationDuration
    {
        get { return (TimeSpan)GetValue(OffsetAnimationDurationProperty); }
        set { SetValue(OffsetAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty PrimaryScaleAnimationDurationProperty =
        DependencyProperty.Register(nameof(PrimaryScaleAnimationDuration), typeof(TimeSpan), typeof(TextGlitchEffect3), new PropertyMetadata(TimeSpan.FromSeconds(1), OnValueChanged));
    public TimeSpan PrimaryScaleAnimationDuration
    {
        get { return (TimeSpan)GetValue(PrimaryScaleAnimationDurationProperty); }
        set { SetValue(PrimaryScaleAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty SecondaryScaleAnimationDurationProperty =
        DependencyProperty.Register(nameof(SecondaryScaleAnimationDuration), typeof(TimeSpan), typeof(TextGlitchEffect3), new PropertyMetadata(TimeSpan.FromSeconds(1.5), OnValueChanged));
    public TimeSpan SecondaryScaleAnimationDuration
    {
        get { return (TimeSpan)GetValue(SecondaryScaleAnimationDurationProperty); }
        set { SetValue(SecondaryScaleAnimationDurationProperty, value); }
    }

    public new static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(TextGlitchEffect3), new PropertyMetadata(90.0, OnValueChanged));
    public new double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly DependencyProperty PrimaryShadowColorProperty =
        DependencyProperty.Register(nameof(PrimaryShadowColor), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Color.FromArgb(230, 255, 0, 0), OnValueChanged));
    public Color PrimaryShadowColor
    {
        get { return (Color)GetValue(PrimaryShadowColorProperty); }
        set { SetValue(PrimaryShadowColorProperty, value); }
    }

    public static readonly DependencyProperty SecondaryShadowColorProperty =
        DependencyProperty.Register(nameof(SecondaryShadowColor), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Color.FromArgb(204, 0, 255, 255), OnValueChanged));
    public Color SecondaryShadowColor
    {
        get { return (Color)GetValue(SecondaryShadowColorProperty); }
        set { SetValue(SecondaryShadowColorProperty, value); }
    }

    public static readonly DependencyProperty TertiaryShadowColorProperty =
        DependencyProperty.Register(nameof(TertiaryShadowColor), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Colors.White, OnValueChanged));
    public Color TertiaryShadowColor
    {
        get { return (Color)GetValue(TertiaryShadowColorProperty); }
        set { SetValue(TertiaryShadowColorProperty, value); }
    }

    public static readonly DependencyProperty PrimaryForegroundProperty =
        DependencyProperty.Register(nameof(PrimaryForeground), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Colors.White, OnValueChanged));
    public Color PrimaryForeground
    {
        get { return (Color)GetValue(PrimaryForegroundProperty); }
        set { SetValue(PrimaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty SecondaryForegroundProperty =
        DependencyProperty.Register(nameof(SecondaryForeground), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Colors.White, OnValueChanged));
    public Color SecondaryForeground
    {
        get { return (Color)GetValue(SecondaryForegroundProperty); }
        set { SetValue(SecondaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty TertiaryForegroundProperty =
        DependencyProperty.Register(nameof(TertiaryForeground), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Colors.White, OnValueChanged));
    public Color TertiaryForeground
    {
        get { return (Color)GetValue(TertiaryForegroundProperty); }
        set { SetValue(TertiaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty PrimaryBackgroundProperty =
        DependencyProperty.Register(nameof(PrimaryBackground), typeof(Brush), typeof(TextGlitchEffect3), new PropertyMetadata(default(SolidColorBrush), OnValueChanged));
    public Brush PrimaryBackground
    {
        get { return (Brush)GetValue(PrimaryBackgroundProperty); }
        set { SetValue(PrimaryBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SecondaryBackgroundProperty =
        DependencyProperty.Register(nameof(SecondaryBackground), typeof(Brush), typeof(TextGlitchEffect3), new PropertyMetadata(default(SolidColorBrush), OnValueChanged));
    public Brush SecondaryBackground
    {
        get { return (Brush)GetValue(SecondaryBackgroundProperty); }
        set { SetValue(SecondaryBackgroundProperty, value); }
    }

    public static readonly DependencyProperty TertiaryBackgroundProperty =
        DependencyProperty.Register(nameof(TertiaryBackground), typeof(Brush), typeof(TextGlitchEffect3), new PropertyMetadata(default(SolidColorBrush), OnValueChanged));
    public Brush TertiaryBackground
    {
        get { return (Brush)GetValue(TertiaryBackgroundProperty); }
        set { SetValue(TertiaryBackgroundProperty, value); }
    }

    public static readonly DependencyProperty PrimaryTextToBrushWrapperProperty =
        DependencyProperty.Register(nameof(PrimaryTextToBrushWrapper), typeof(TextToBrushWrapper), typeof(TextGlitchEffect3), new PropertyMetadata(null, OnValueChanged));
    public TextToBrushWrapper PrimaryTextToBrushWrapper
    {
        get { return (TextToBrushWrapper)GetValue(PrimaryTextToBrushWrapperProperty); }
        set { SetValue(PrimaryTextToBrushWrapperProperty, value); }
    }

    public static readonly DependencyProperty SecondaryTextToBrushWrapperProperty =
        DependencyProperty.Register(nameof(SecondaryTextToBrushWrapper), typeof(TextToBrushWrapper), typeof(TextGlitchEffect3), new PropertyMetadata(null, OnValueChanged));
    public TextToBrushWrapper SecondaryTextToBrushWrapper
    {
        get { return (TextToBrushWrapper)GetValue(SecondaryTextToBrushWrapperProperty); }
        set { SetValue(SecondaryTextToBrushWrapperProperty, value); }
    }

    public static readonly DependencyProperty TertiaryTextToBrushWrapperProperty =
        DependencyProperty.Register(nameof(TertiaryTextToBrushWrapper), typeof(TextToBrushWrapper), typeof(TextGlitchEffect3), new PropertyMetadata(null, OnValueChanged));
    public TextToBrushWrapper TertiaryTextToBrushWrapper
    {
        get { return (TextToBrushWrapper)GetValue(TertiaryTextToBrushWrapperProperty); }
        set { SetValue(TertiaryTextToBrushWrapperProperty, value); }
    }

    public static readonly DependencyProperty BlendEffectProperty =
        DependencyProperty.Register(nameof(BlendEffect), typeof(BlendEffectMode), typeof(TextGlitchEffect3), new PropertyMetadata(BlendEffectMode.Multiply, OnValueChanged));
    public BlendEffectMode BlendEffect
    {
        get { return (BlendEffectMode)GetValue(BlendEffectProperty); }
        set { SetValue(BlendEffectProperty, value); }
    }

    public static readonly DependencyProperty LineHeightProperty =
        DependencyProperty.Register(nameof(LineHeight), typeof(double), typeof(TextGlitchEffect3), new PropertyMetadata(6.0, OnValueChanged));
    public double LineHeight
    {
        get { return (double)GetValue(LineHeightProperty); }
        set { SetValue(LineHeightProperty, value); }
    }

    public static readonly DependencyProperty LineColorProperty =
        DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(TextGlitchEffect3), new PropertyMetadata(Colors.Black, OnValueChanged));
    public Color LineColor
    {
        get { return (Color)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }
}
