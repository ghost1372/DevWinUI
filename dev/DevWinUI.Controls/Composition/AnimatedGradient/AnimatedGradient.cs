using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = nameof(PART_Rectangle), Type = typeof(Rectangle))]
public partial class AnimatedGradient : Control
{
    private const string PART_Rectangle = "PART_Rectangle";
    private Rectangle rectangle;

    private CompositionColorGradientStop _gradientStop1;
    private CompositionColorGradientStop _gradientStop2;
    private SpriteVisual visual;

    private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();

    public AnimatedGradient()
    {
        this.DefaultStyleKey = typeof(AnimatedGradient);
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
            new PropertyMetadata(Colors.DeepPink));

    public Color WarmColor2
    {
        get => (Color)GetValue(WarmColor2Property);
        set => SetValue(WarmColor2Property, value);
    }

    public static readonly DependencyProperty WarmColor2Property =
        DependencyProperty.Register(nameof(WarmColor2), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.Honeydew));

    public Color CoolColor1
    {
        get => (Color)GetValue(CoolColor1Property);
        set => SetValue(CoolColor1Property, value);
    }

    public static readonly DependencyProperty CoolColor1Property =
        DependencyProperty.Register(nameof(CoolColor1), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.LightSkyBlue));

    public Color CoolColor2
    {
        get => (Color)GetValue(CoolColor2Property);
        set => SetValue(CoolColor2Property, value);
    }

    public static readonly DependencyProperty CoolColor2Property =
        DependencyProperty.Register(nameof(CoolColor2), typeof(Color), typeof(AnimatedGradient),
            new PropertyMetadata(Colors.Teal));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AnimatedGradient)d;
        ctl?.InitAnimation();
    }

    private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AnimatedGradient)d;
        ctl?.ApplyColorScheme();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        rectangle = GetTemplateChild(PART_Rectangle) as Rectangle;

        PointerEntered -= OnPointerEntered;
        PointerEntered += OnPointerEntered;

        PointerPressed -= OnPointerPressed;
        PointerPressed += OnPointerPressed;

        InitAnimation();
    }

    private void InitAnimation()
    {
        if (Content == null || rectangle == null)
            return;

        var (stop1Color, stop2Color) = GetCurrentColors();

        var linearGradientBrush = _compositor.CreateLinearGradientBrush();

        _gradientStop1 = _compositor.CreateColorGradientStop();
        _gradientStop1.Color = stop1Color;

        _gradientStop2 = _compositor.CreateColorGradientStop();
        _gradientStop2.Offset = 1;
        _gradientStop2.Color = stop2Color;

        linearGradientBrush.ColorStops.Add(_gradientStop1);
        linearGradientBrush.ColorStops.Add(_gradientStop2);

        visual = _compositor.CreateSpriteVisual();
        visual.Brush = linearGradientBrush;
        visual.Scale = new Vector3(0, 1, 1);
        visual.RelativeSizeAdjustment = Vector2.One;

        ElementCompositionPreview.SetElementChildVisual(rectangle, visual);
    }

    private void ApplyColorScheme()
    {
        if (_gradientStop1 == null || _gradientStop2 == null)
            return;

        var (stop1Color, stop2Color) = GetCurrentColors();
        _gradientStop1.StartAnimation("Color", CreateAnimationToColor(stop1Color));
        _gradientStop2.StartAnimation("Color", CreateAnimationToColor(stop2Color));
    }

    private ColorKeyFrameAnimation CreateAnimationToColor(Color color)
    {
        var animation = _compositor.CreateColorKeyFrameAnimation();
        animation.InsertKeyFrame(1, color);
        animation.Duration = TimeSpan.FromSeconds(2);
        return animation;
    }

    private void AnimateToNewPosition(Visual visual)
    {
        var scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0, new Vector3(0, 1, 1));
        scaleAnimation.InsertKeyFrame(0.5f, Vector3.One);
        scaleAnimation.InsertKeyFrame(1, new Vector3(0, 1, 1));
        scaleAnimation.Duration = TimeSpan.FromSeconds(2);
        visual.StartAnimation("Scale", scaleAnimation);

        var targetX = visual.RelativeOffsetAdjustment.X == 0 ? 1 : 0;
        visual.AnchorPoint = new Vector2(targetX, 0);

        var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
        offsetAnimation.Duration = TimeSpan.FromSeconds(1);
        offsetAnimation.InsertKeyFrame(1, targetX);
        visual.StartAnimation("RelativeOffsetAdjustment.X", offsetAnimation);
    }

    private void OnPointerPressed(object sender, RoutedEventArgs e)
    {
        ColorScheme = GetComplementaryColorScheme(ColorScheme);
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (Content == null || visual == null)
            return;

        AnimateToNewPosition(visual);
        ColorScheme = GetReversedColorScheme(ColorScheme);
    }

    private (Color, Color) GetCurrentColors()
    {
        return ColorScheme switch
        {
            AnimatedGradientColorScheme.Warm => (WarmColor1, WarmColor2),
            AnimatedGradientColorScheme.WarmReversed => Reverse((WarmColor1, WarmColor2)),
            AnimatedGradientColorScheme.Cool => (CoolColor1, CoolColor2),
            AnimatedGradientColorScheme.CoolReversed => Reverse((CoolColor1, CoolColor2)),
            _ => throw new InvalidOperationException()
        };
    }

    private static AnimatedGradientColorScheme GetComplementaryColorScheme(AnimatedGradientColorScheme colorScheme) =>
        ApplyColorSchemeFunction(
            colorScheme,
            ifWarm: AnimatedGradientColorScheme.Cool,
            ifWarmReversed: AnimatedGradientColorScheme.CoolReversed,
            ifCool: AnimatedGradientColorScheme.Warm,
            ifCoolReversed: AnimatedGradientColorScheme.WarmReversed);

    private static AnimatedGradientColorScheme GetReversedColorScheme(AnimatedGradientColorScheme colorScheme) =>
        ApplyColorSchemeFunction(
            colorScheme,
            ifWarm: AnimatedGradientColorScheme.WarmReversed,
            ifWarmReversed: AnimatedGradientColorScheme.Warm,
            ifCool: AnimatedGradientColorScheme.CoolReversed,
            ifCoolReversed: AnimatedGradientColorScheme.Cool);

    private static AnimatedGradientColorScheme ApplyColorSchemeFunction(
        AnimatedGradientColorScheme input,
        AnimatedGradientColorScheme ifWarm,
        AnimatedGradientColorScheme ifWarmReversed,
        AnimatedGradientColorScheme ifCool,
        AnimatedGradientColorScheme ifCoolReversed)
    {
        return input switch
        {
            AnimatedGradientColorScheme.Warm => ifWarm,
            AnimatedGradientColorScheme.WarmReversed => ifWarmReversed,
            AnimatedGradientColorScheme.Cool => ifCool,
            AnimatedGradientColorScheme.CoolReversed => ifCoolReversed,
            _ => throw new InvalidOperationException()
        };
    }

    private static (T, T) Reverse<T>((T, T) pair) => (pair.Item2, pair.Item1);
}
