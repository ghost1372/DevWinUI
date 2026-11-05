// https://github.com/DinoChan

using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextBackground), Type = typeof(Rectangle))]
public partial class TextGlitchEffect2 : Control
{
    private static readonly List<(double, double)> PrimaryHeightKeyFrames = new() { (0, 1), (20, 80), (60, 15), (100, 105) };
    private static readonly List<(double, double)> SecondaryHeightKeyFrames = new() { (0, 110), (20, 112.5), (35, 30), (50, 100), (60, 50), (70, 85), (80, 55), (100, 1) };

    private string PART_TextBackground = "PART_TextBackground";
    private Rectangle _rectangle;
    private Compositor _compositor;
    private Compositor Compositor => _compositor ??= ElementCompositionPreview.GetElementVisual(this).Compositor;
    private TextToBrushWrapper primaryWrapper;
    private TextToBrushWrapper secondaryWrapper;
    private TextToBrushWrapper tertiaryWrapper;
    private SpriteVisual lineVisual;
    private Storyboard _storyboard1;
    private Storyboard _storyboard2;
    private Size _lastSize;
    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextGlitchEffect2)d;
        if (ctl != null)
        {
            ctl.UpdateTextEffect();
        }
    }

    public TextGlitchEffect2()
    {
        DefaultStyleKey = typeof(TextGlitchEffect2);

        if (PrimaryBackground == null)
        {
            PrimaryBackground = new SolidColorBrush(Colors.Black);
        }

        if (SecondaryBackground == null)
        {
            SecondaryBackground = new SolidColorBrush(Colors.Black);
        }

        if (TertiaryBackground == null)
        {
            TertiaryBackground = new SolidColorBrush(Colors.Black);
        }
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (_rectangle != null)
        {
            _rectangle.SizeChanged -= TextGlitchEffect_SizeChanged;
        }

        _rectangle = GetTemplateChild(PART_TextBackground) as Rectangle;
        if (_rectangle != null)
        {
            Loaded -= TextGlitchEffect_Loaded;
            Loaded += TextGlitchEffect_Loaded;
            Unloaded -= TextGlitchEffect_Unloaded;
            Unloaded += TextGlitchEffect_Unloaded;
            _rectangle.SizeChanged -= TextGlitchEffect_SizeChanged;
            _rectangle.SizeChanged += TextGlitchEffect_SizeChanged;
            SizeChanged -= TextGlitchEffect_SizeChanged;
            SizeChanged += TextGlitchEffect_SizeChanged;
        }
    }

    public Rectangle GetRectangle()
    {
        return _rectangle;
    }

    private void TextGlitchEffect_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (RenderSize == _lastSize)
        {
            return;
        }

        UpdateTextEffect();
    }

    private void UpdateTextEffect()
    {
        if (_rectangle == null || RenderSize.Width <= 0 || RenderSize.Height <= 0)
        {
            return;
        }

        StopAnimation(lineVisual);

        if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(PrimaryTextToBrushWrapper?.Text) && string.IsNullOrEmpty(SecondaryTextToBrushWrapper?.Text) && string.IsNullOrEmpty(TertiaryTextToBrushWrapper?.Text))
        {
            ElementCompositionPreview.SetElementChildVisual(_rectangle, null);
            _lastSize = default;
            return;
        }

        primaryWrapper = PrimaryTextToBrushWrapper ?? CreateTextToBrushWrapper(PrimaryForeground, PrimaryBackground, 2, PrimaryShadowColor);

        if (SecondaryTextToBrushWrapper != null)
        {
            secondaryWrapper = SecondaryTextToBrushWrapper;
        }
        else
        {
            secondaryWrapper = CreateTextToBrushWrapper(SecondaryForeground, SecondaryBackground, -2, SecondaryShadowColor);
            secondaryWrapper.Brush.Offset = new Vector2(-7f, 0);
        }

        tertiaryWrapper = TertiaryTextToBrushWrapper ?? CreateTextToBrushWrapper(TertiaryForeground, TertiaryBackground, 0, TertiaryShadowColor);
        _lastSize = RenderSize;

        var size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(RenderSize.Height));

        var containerVisual = Compositor.CreateContainerVisual();
        var foregroundVisual = Compositor.CreateSpriteVisual();
        foregroundVisual.Brush = CreateBrush(secondaryWrapper.Brush, primaryWrapper.Brush, BlendEffect);
        foregroundVisual.Size = size;
        containerVisual.Children.InsertAtBottom(foregroundVisual);

        var textVisual = Compositor.CreateSpriteVisual();
        textVisual.Brush = tertiaryWrapper.Brush;
        textVisual.Size = size;
        containerVisual.Children.InsertAtBottom(textVisual);

        lineVisual = Compositor.CreateSpriteVisual();
        lineVisual.Brush = Compositor.CreateColorBrush(LineColor);
        lineVisual.Size = new Vector2(size.X, Convert.ToSingle(LineHeight));
        containerVisual.Children.InsertAtTop(lineVisual);

        ElementCompositionPreview.SetElementChildVisual(_rectangle, containerVisual);

        _storyboard1 = StartHeightAnimation(primaryWrapper, PrimaryHeightKeyFrames, TimeSpan.FromSeconds(1), TimeSpan.Zero);
        _storyboard2 = StartHeightAnimation(secondaryWrapper, SecondaryHeightKeyFrames, TimeSpan.FromSeconds(1.5), TimeSpan.Zero);
        StartOffsetAnimation(lineVisual, TimeSpan.FromSeconds(3), TimeSpan.Zero);
    }

    private void TextGlitchEffect_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTextEffect();
    }

    private void TextGlitchEffect_Unloaded(object sender, RoutedEventArgs e)
    {
        StopAnimation(lineVisual);
    }

    public TextToBrushWrapper CreateTextToBrushWrapper(Color fontColor, Brush background, double shadowOffsetX, Color shadowColor)
    {
        var result = new TextToBrushWrapper
        {
            Text = Text,
            FontSize = FontSize,
            FontColor = fontColor,
            Background = background,
            ShadowBlurAmount = 0,
            ShadowOffsetX = shadowOffsetX,
            ShadowColor = shadowColor,
            Width = Convert.ToSingle(RenderSize.Width),
            Height = Convert.ToSingle(RenderSize.Height)
        };
        result.Brush.VerticalAlignmentRatio = 0;
        return result;
    }

    private CompositionBrush CreateBrush(CompositionBrush foreground, CompositionBrush background, BlendEffectMode blendEffectMode)
    {
        var compositor = Compositor;
        var effect = new BlendEffect
        {
            Mode = blendEffectMode,
            Foreground = new CompositionEffectSourceParameter("Main"),
            Background = new CompositionEffectSourceParameter("Tint")
        };
        var effectFactory = compositor.CreateEffectFactory(effect);
        var compositionBrush = effectFactory.CreateBrush();
        compositionBrush.SetSourceParameter("Main", foreground);
        compositionBrush.SetSourceParameter("Tint", background);

        return compositionBrush;
    }

    private Storyboard StartHeightAnimation(TextToBrushWrapper brush, List<(double, double)> keyFrames, TimeSpan duration, TimeSpan delay)
    {
        var storyboard = new Storyboard();
        var animation = new DoubleAnimationUsingKeyFrames();
        animation.EnableDependentAnimation = true;
        Storyboard.SetTarget(animation, brush);
        Storyboard.SetTargetProperty(animation, nameof(Height));

        foreach (var item in keyFrames)
            animation.KeyFrames.Add(new LinearDoubleKeyFrame
            { KeyTime = duration / 100 * item.Item1, Value = item.Item2 });

        storyboard.Children.Add(animation);
        storyboard.RepeatBehavior = RepeatBehavior.Forever;

        storyboard.BeginTime = delay;
        storyboard.Begin();
        return storyboard;
    }

    private void StartOffsetAnimation(SpriteVisual visual, TimeSpan duration, TimeSpan delay)
    {
        var offsetAnimation = Compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Duration = duration;
        offsetAnimation.DelayTime = delay;
        offsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
        var easing = Compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.2f, 1f));

        void addKey(float key, float top)
        {
            offsetAnimation.InsertKeyFrame(key, new Vector3(0, top, 0), easing);
        }

        addKey(.08f, 95);
        addKey(.14f, 20);
        addKey(.20f, 105);
        addKey(.32f, 5);
        addKey(.99f, 75);
        visual.StartAnimation(nameof(CompositionSurfaceBrush.Offset), offsetAnimation);
    }

    private void StopAnimation(SpriteVisual visual)
    {
        if (visual != null)
        {
            visual.StopAnimation(nameof(CompositionSurfaceBrush.Offset));
        }

        if (_storyboard1 != null)
        {
            _storyboard1.Stop();
        }

        if (_storyboard2 != null)
        {
            _storyboard2.Stop();
        }
    }
}
