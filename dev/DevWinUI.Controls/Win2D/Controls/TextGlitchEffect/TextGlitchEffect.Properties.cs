// https://github.com/DinoChan

namespace DevWinUI;
public partial class TextGlitchEffect
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextGlitchEffect), new PropertyMetadata(null, OnValueChanged));
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public new static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(TextGlitchEffect), new PropertyMetadata(72.0, OnValueChanged));
    public new double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly DependencyProperty PrimaryForegroundProperty =
        DependencyProperty.Register(nameof(PrimaryForeground), typeof(Color), typeof(TextGlitchEffect), new PropertyMetadata(Colors.Cyan, OnValueChanged));
    public Color PrimaryForeground
    {
        get { return (Color)GetValue(PrimaryForegroundProperty); }
        set { SetValue(PrimaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty SecondaryForegroundProperty =
        DependencyProperty.Register(nameof(SecondaryForeground), typeof(Color), typeof(TextGlitchEffect), new PropertyMetadata(Colors.Red, OnValueChanged));
    public Color SecondaryForeground
    {
        get { return (Color)GetValue(SecondaryForegroundProperty); }
        set { SetValue(SecondaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty PrimaryBackgroundProperty =
        DependencyProperty.Register(nameof(PrimaryBackground), typeof(Brush), typeof(TextGlitchEffect), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnValueChanged));
    public Brush PrimaryBackground
    {
        get { return (Brush)GetValue(PrimaryBackgroundProperty); }
        set { SetValue(PrimaryBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SecondaryBackgroundProperty =
        DependencyProperty.Register(nameof(SecondaryBackground), typeof(Brush), typeof(TextGlitchEffect), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnValueChanged));
    public Brush SecondaryBackground
    {
        get { return (Brush)GetValue(SecondaryBackgroundProperty); }
        set { SetValue(SecondaryBackgroundProperty, value); }
    }

    public static readonly DependencyProperty PrimaryTextToBrushWrapperProperty =
        DependencyProperty.Register(nameof(PrimaryTextToBrushWrapper), typeof(TextToBrushWrapper), typeof(TextGlitchEffect), new PropertyMetadata(null, OnValueChanged));
    public TextToBrushWrapper PrimaryTextToBrushWrapper
    {
        get { return (TextToBrushWrapper)GetValue(PrimaryTextToBrushWrapperProperty); }
        set { SetValue(PrimaryTextToBrushWrapperProperty, value); }
    }

    public static readonly DependencyProperty SecondaryTextToBrushWrapperProperty =
        DependencyProperty.Register(nameof(SecondaryTextToBrushWrapper), typeof(TextToBrushWrapper), typeof(TextGlitchEffect), new PropertyMetadata(null, OnValueChanged));
    public TextToBrushWrapper SecondaryTextToBrushWrapper
    {
        get { return (TextToBrushWrapper)GetValue(SecondaryTextToBrushWrapperProperty); }
        set { SetValue(SecondaryTextToBrushWrapperProperty, value); }
    }

    public static readonly DependencyProperty BlendEffectProperty =
        DependencyProperty.Register(nameof(BlendEffect), typeof(BlendEffectMode), typeof(TextGlitchEffect), new PropertyMetadata(BlendEffectMode.Lighten, OnValueChanged));
    public BlendEffectMode BlendEffect
    {
        get { return (BlendEffectMode)GetValue(BlendEffectProperty); }
        set { SetValue(BlendEffectProperty, value); }
    }

    public static readonly DependencyProperty LineHeightProperty =
        DependencyProperty.Register(nameof(LineHeight), typeof(double), typeof(TextGlitchEffect), new PropertyMetadata(2.0, OnValueChanged));
    public double LineHeight
    {
        get { return (double)GetValue(LineHeightProperty); }
        set { SetValue(LineHeightProperty, value); }
    }

    public static readonly DependencyProperty LineColorProperty =
        DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(TextGlitchEffect), new PropertyMetadata(Colors.Black, OnValueChanged));
    public Color LineColor
    {
        get { return (Color)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }
}
