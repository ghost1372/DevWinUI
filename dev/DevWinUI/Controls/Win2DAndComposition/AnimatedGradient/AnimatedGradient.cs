using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = PART_Rectangle, Type = typeof(Rectangle))]
public partial class AnimatedGradient : Control
{
    private const string PART_Rectangle = "PART_Rectangle";

    private static readonly TimeSpan ColorDuration = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan OffsetDuration = TimeSpan.FromSeconds(1);

    private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();
    private readonly Vector3KeyFrameAnimation _scaleAnimation;

    private Rectangle _rectangle;
    private SpriteVisual _visual;
    private CompositionLinearGradientBrush _brush;
    private CompositionColorGradientStop _gradientStop1;
    private CompositionColorGradientStop _gradientStop2;
    private bool _isAtEnd;

    public AnimatedGradient()
    {
        this.DefaultStyleKey = typeof(AnimatedGradient);

        _scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
        _scaleAnimation.InsertKeyFrame(0, new Vector3(0, 1, 1));
        _scaleAnimation.InsertKeyFrame(0.5f, Vector3.One);
        _scaleAnimation.InsertKeyFrame(1, new Vector3(0, 1, 1));
        _scaleAnimation.Duration = ColorDuration;
    }

    public FrameworkElement Content
    {
        get => (FrameworkElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(AnimatedGradient), new PropertyMetadata(null, OnContentChanged));

    public AnimatedGradientColorScheme ColorScheme
    {
        get => (AnimatedGradientColorScheme)GetValue(ColorSchemeProperty);
        set => SetValue(ColorSchemeProperty, value);
    }

    public static readonly DependencyProperty ColorSchemeProperty =
        DependencyProperty.Register(nameof(ColorScheme), typeof(AnimatedGradientColorScheme), typeof(AnimatedGradient),
            new PropertyMetadata(AnimatedGradientColorScheme.Warm, OnColorSchemeChanged));

    public Color WarmColor1
    {
        get => (Color)GetValue(WarmColor1Property);
        set => SetValue(WarmColor1Property, value);
    }

    public static readonly DependencyProperty WarmColor1Property =
        DependencyProperty.Register(nameof(WarmColor1), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.DeepPink, OnColorSchemeChanged));

    public Color WarmColor2
    {
        get => (Color)GetValue(WarmColor2Property);
        set => SetValue(WarmColor2Property, value);
    }

    public static readonly DependencyProperty WarmColor2Property =
        DependencyProperty.Register(nameof(WarmColor2), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.Honeydew, OnColorSchemeChanged));

    public Color CoolColor1
    {
        get => (Color)GetValue(CoolColor1Property);
        set => SetValue(CoolColor1Property, value);
    }

    public static readonly DependencyProperty CoolColor1Property =
        DependencyProperty.Register(nameof(CoolColor1), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.LightSkyBlue, OnColorSchemeChanged));

    public Color CoolColor2
    {
        get => (Color)GetValue(CoolColor2Property);
        set => SetValue(CoolColor2Property, value);
    }

    public static readonly DependencyProperty CoolColor2Property =
        DependencyProperty.Register(nameof(CoolColor2), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.Teal, OnColorSchemeChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AnimatedGradient)d;
        ctl.InitAnimation();
    }

    private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AnimatedGradient)d;
        ctl.ApplyColorScheme();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ReleaseVisual();
        _rectangle = GetTemplateChild(PART_Rectangle) as Rectangle;

        InitAnimation();
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);

        if (_visual == null)
            return;

        AnimateToNewPosition();
        ColorScheme = GetReversedColorScheme(ColorScheme);
    }

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);

        ColorScheme = GetComplementaryColorScheme(ColorScheme);
    }

    private void InitAnimation()
    {
        ReleaseVisual();

        if (Content == null || _rectangle == null)
            return;

        var (stop1Color, stop2Color) = GetCurrentColors();

        _gradientStop1 = _compositor.CreateColorGradientStop(0, stop1Color);
        _gradientStop2 = _compositor.CreateColorGradientStop(1, stop2Color);

        _brush = _compositor.CreateLinearGradientBrush();
        _brush.ColorStops.Add(_gradientStop1);
        _brush.ColorStops.Add(_gradientStop2);

        _visual = _compositor.CreateSpriteVisual();
        _visual.Brush = _brush;
        _visual.Scale = new Vector3(0, 1, 1);
        _visual.RelativeSizeAdjustment = Vector2.One;

        ElementCompositionPreview.SetElementChildVisual(_rectangle, _visual);
    }

    private void ReleaseVisual()
    {
        if (_visual == null)
            return;

        if (_rectangle != null)
            ElementCompositionPreview.SetElementChildVisual(_rectangle, null);

        _visual.Dispose();
        _brush.Dispose();
        _gradientStop1.Dispose();
        _gradientStop2.Dispose();

        _visual = null;
        _brush = null;
        _gradientStop1 = null;
        _gradientStop2 = null;
        _isAtEnd = false;
    }

    private void ApplyColorScheme()
    {
        if (_gradientStop1 == null)
            return;

        var (stop1Color, stop2Color) = GetCurrentColors();
        _gradientStop1.StartAnimation(nameof(CompositionColorGradientStop.Color), CreateAnimationToColor(stop1Color));
        _gradientStop2.StartAnimation(nameof(CompositionColorGradientStop.Color), CreateAnimationToColor(stop2Color));
    }

    private ColorKeyFrameAnimation CreateAnimationToColor(Color color)
    {
        var animation = _compositor.CreateColorKeyFrameAnimation();
        animation.InsertKeyFrame(1, color);
        animation.Duration = ColorDuration;
        return animation;
    }

    private void AnimateToNewPosition()
    {
        _visual.StartAnimation(nameof(Visual.Scale), _scaleAnimation);

        _isAtEnd = !_isAtEnd;
        var targetX = _isAtEnd ? 1f : 0f;
        _visual.AnchorPoint = new Vector2(targetX, 0);

        var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
        offsetAnimation.Duration = OffsetDuration;
        offsetAnimation.InsertKeyFrame(1, targetX);
        _visual.StartAnimation("RelativeOffsetAdjustment.X", offsetAnimation);
    }

    private (Color, Color) GetCurrentColors() => ColorScheme switch
    {
        AnimatedGradientColorScheme.Warm => (WarmColor1, WarmColor2),
        AnimatedGradientColorScheme.WarmReversed => (WarmColor2, WarmColor1),
        AnimatedGradientColorScheme.Cool => (CoolColor1, CoolColor2),
        AnimatedGradientColorScheme.CoolReversed => (CoolColor2, CoolColor1),
        _ => (WarmColor1, WarmColor2)
    };

    private static AnimatedGradientColorScheme GetComplementaryColorScheme(AnimatedGradientColorScheme colorScheme) => colorScheme switch
    {
        AnimatedGradientColorScheme.Warm => AnimatedGradientColorScheme.Cool,
        AnimatedGradientColorScheme.WarmReversed => AnimatedGradientColorScheme.CoolReversed,
        AnimatedGradientColorScheme.Cool => AnimatedGradientColorScheme.Warm,
        _ => AnimatedGradientColorScheme.WarmReversed
    };

    private static AnimatedGradientColorScheme GetReversedColorScheme(AnimatedGradientColorScheme colorScheme) => colorScheme switch
    {
        AnimatedGradientColorScheme.Warm => AnimatedGradientColorScheme.WarmReversed,
        AnimatedGradientColorScheme.WarmReversed => AnimatedGradientColorScheme.Warm,
        AnimatedGradientColorScheme.Cool => AnimatedGradientColorScheme.CoolReversed,
        _ => AnimatedGradientColorScheme.Cool
    };
}
