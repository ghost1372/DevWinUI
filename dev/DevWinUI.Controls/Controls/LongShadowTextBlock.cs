using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ShadowElement), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_TextBlock), Type = typeof(ContentPresenter))]
[ContentProperty(Name = nameof(TextBlock))]
public partial class LongShadowTextBlock : Control
{
    private string PART_ShadowElement = "PART_ShadowElement";
    private string PART_TextBlock = "PART_TextBlock";
    private Rectangle _shadowElement;
    private long _textblockTextChanged;
    public TextBlock TextBlock
    {
        get { return (TextBlock)GetValue(TextBlockProperty); }
        set { SetValue(TextBlockProperty, value); }
    }

    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public int Depth
    {
        get { return (int)GetValue(DepthProperty); }
        set { SetValue(DepthProperty, value); }
    }

    public double TextOpacity
    {
        get { return (double)GetValue(TextOpacityProperty); }
        set { SetValue(TextOpacityProperty, value); }
    }

    public static readonly DependencyProperty TextBlockProperty =
        DependencyProperty.Register(nameof(TextBlock), typeof(TextBlock), typeof(LongShadowTextBlock), new PropertyMetadata(default(TextBlock)));

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(int), typeof(LongShadowTextBlock), new PropertyMetadata(Color.FromArgb(255, 250, 110, 93), OnColorChanged));

    public static readonly DependencyProperty DepthProperty =
        DependencyProperty.Register(nameof(Depth), typeof(int), typeof(LongShadowTextBlock), new PropertyMetadata(100, OnDepthChanged));

    public static readonly DependencyProperty TextOpacityProperty =
        DependencyProperty.Register(nameof(TextOpacity), typeof(double), typeof(LongShadowTextBlock), new PropertyMetadata(0.3, OnTextOpacityChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LongShadowTextBlock)d;
        if (ctl != null)
        {
            CompositionHelper.MakeLongShadow(ctl.Depth, Convert.ToSingle(ctl.TextOpacity), ctl.TextBlock, ctl._shadowElement, ctl.Color);
        }
    }

    private static void OnDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LongShadowTextBlock)d;
        if (ctl != null)
        {
            CompositionHelper.MakeLongShadow(ctl.Depth, Convert.ToSingle(ctl.TextOpacity), ctl.TextBlock, ctl._shadowElement, ctl.Color);
        }
    }

    private static void OnTextOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LongShadowTextBlock)d;
        if (ctl != null)
        {
            CompositionHelper.MakeLongShadow(ctl.Depth, Convert.ToSingle(ctl.TextOpacity), ctl.TextBlock, ctl._shadowElement, ctl.Color);
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _shadowElement = GetTemplateChild(PART_ShadowElement) as Rectangle;
        SizeChanged -= LongShadowTextBlock_SizeChanged;
        SizeChanged += LongShadowTextBlock_SizeChanged;

        UnregisterPropertyChangedCallback(TextBlock.TextProperty, _textblockTextChanged);
        _textblockTextChanged = RegisterPropertyChangedCallback(TextBlock.TextProperty, OnTextBlockTextChanged);

        CompositionHelper.MakeLongShadow(Depth, Convert.ToSingle(TextOpacity), TextBlock, _shadowElement, Color);
    }

    private void LongShadowTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        CompositionHelper.MakeLongShadow(Depth, Convert.ToSingle(TextOpacity), TextBlock, _shadowElement, Color);
    }

    private void OnTextBlockTextChanged(DependencyObject sender, DependencyProperty dp)
    {
        CompositionHelper.MakeLongShadow(Depth, Convert.ToSingle(TextOpacity), TextBlock, _shadowElement, Color);
    }
}
