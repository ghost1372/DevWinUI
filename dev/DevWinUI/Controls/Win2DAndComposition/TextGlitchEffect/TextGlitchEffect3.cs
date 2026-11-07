// https://github.com/DinoChan

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextBackground), Type = typeof(Rectangle))]
public partial class TextGlitchEffect3 : Control
{
    private static readonly List<(double, double)> HeightKeyFrames = new() { (18, 110), (20, 112.5), (25, 110) };
    private static readonly List<(float, float)> PrimaryScaleKeyFrames = new() { (0, 0.01f), (.20f, .73f), (.60f, .14f), (1, .95f) };
    private static readonly List<(float, float)> SecondaryScaleKeyFrames = new() { (0, 1), (.20f, 1), (.35f, .27f), (.50f, .91f), (.60f, .45f), (.70f, .77f), (.80f, .5f), (1, 0) };

    private string PART_TextBackground = "PART_TextBackground";
    private Rectangle _rectangle;
    private Compositor _compositor;
    private Compositor Compositor => _compositor ??= ElementCompositionPreview.GetElementVisual(this).Compositor;
    private TextToBrushWrapper primaryWrapper;
    private TextToBrushWrapper secondaryWrapper;
    private TextToBrushWrapper tertiaryWrapper;
    private SpriteVisual lineVisual;
    private CompositionSurfaceBrush redMaskBrush;
    private CompositionSurfaceBrush blueMaskBrush;
    private LoadedImageSurface _maskSurface;
    private Storyboard _storyboard;
    private CancellationTokenSource _cancellationTokenSource;
    private Size _lastSize;
    private static async void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextGlitchEffect3)d;
        if (ctl != null)
        {
            await ctl.UpdateTextEffectAsync();
        }
    }

    public TextGlitchEffect3()
    {
        DefaultStyleKey = typeof(TextGlitchEffect3);

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

    private async void TextGlitchEffect_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (RenderSize == _lastSize)
        {
            return;
        }

        await UpdateTextEffectAsync();
    }

    private async Task UpdateTextEffectAsync()
    {
        if (_rectangle == null || RenderSize.Width <= 0 || RenderSize.Height <= 0)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        StopAnimation(lineVisual);
        StopAnimation(redMaskBrush);
        StopAnimation(blueMaskBrush);

        if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(PrimaryTextToBrushWrapper?.Text) && string.IsNullOrEmpty(SecondaryTextToBrushWrapper?.Text) && string.IsNullOrEmpty(TertiaryTextToBrushWrapper?.Text))
        {
            ElementCompositionPreview.SetElementChildVisual(_rectangle, null);
            _lastSize = default;
            return;
        }

        primaryWrapper = PrimaryTextToBrushWrapper ?? CreateTextToBrushWrapper(PrimaryForeground, PrimaryBackground, 3, PrimaryShadowColor);

        if (SecondaryTextToBrushWrapper != null)
        {
            secondaryWrapper = SecondaryTextToBrushWrapper;
        }
        else
        {
            secondaryWrapper = CreateTextToBrushWrapper(SecondaryForeground, SecondaryBackground, -3, SecondaryShadowColor);
            secondaryWrapper.Brush.Offset = new Vector2(-7f, 0);
        }

        tertiaryWrapper = TertiaryTextToBrushWrapper ?? CreateTextToBrushWrapper(TertiaryForeground, TertiaryBackground, 0, TertiaryShadowColor);
        _lastSize = RenderSize;

        var size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(RenderSize.Height));

        (var redBrush, redMaskBrush) = CreateMaskedBrush(primaryWrapper.Brush);
        (var blueBrush, blueMaskBrush) = CreateMaskedBrush(secondaryWrapper.Brush);

        var containerVisual = Compositor.CreateContainerVisual();
        var foregroundVisual = Compositor.CreateSpriteVisual();

        foregroundVisual.Brush = CreateBrush(blueBrush, redBrush, BlendEffect);
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

        _storyboard = StartHeightAnimation(secondaryWrapper, HeightKeyFrames, StoryBoardAnimationDuration, TimeSpan.Zero);
        StartOffseteAnimation(lineVisual, OffsetAnimationDuration, TimeSpan.Zero);
        StartScaleAnimation(redMaskBrush, PrimaryScaleKeyFrames, PrimaryScaleAnimationDuration, TimeSpan.Zero);
        StartScaleAnimation(blueMaskBrush, SecondaryScaleKeyFrames, SecondaryScaleAnimationDuration, TimeSpan.Zero);

        var primary = primaryWrapper;
        var secondary = secondaryWrapper;
        var tertiary = tertiaryWrapper;
        var delay = Delay;
        var words = (primary.Text ?? string.Empty).Split(Delimiter);
        var cancellationTokenSource = _cancellationTokenSource = new CancellationTokenSource();
        var index = 0;

        try
        {
            while (true)
            {
                var word = words[index % words.Length];
                primary.Text = word;
                secondary.Text = word;
                tertiary.Text = word;

                if (words.Length == 1 || delay <= TimeSpan.Zero)
                {
                    break;
                }

                await Task.Delay(delay, cancellationTokenSource.Token);
                index++;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async void TextGlitchEffect_Loaded(object sender, RoutedEventArgs e)
    {
        await UpdateTextEffectAsync();
    }

    private void TextGlitchEffect_Unloaded(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        StopAnimation(lineVisual);
        StopAnimation(redMaskBrush);
        StopAnimation(blueMaskBrush);
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
            FontFamily = new FontFamily("Arial, Helvetica"),
            FontWeight = FontWeights.Thin,
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

    private (CompositionBrush, CompositionSurfaceBrush) CreateMaskedBrush(CompositionBrush source)
    {
        var compositor = Compositor;
        var effect = new AlphaMaskEffect
        {
            Source = new CompositionEffectSourceParameter("Source"),
            AlphaMask = new CompositionEffectSourceParameter("Mask")
        };

        if (_maskSurface == null)
        {
            var assembly = typeof(TextGlitchEffect3).Assembly;
            var stream = FileHelper.GetFileFromEmbededResourcesOrUri(new Uri("ms-appx:///Assets/Mask/TextGlitchMask.png"), assembly);
            _maskSurface = LoadedImageSurface.StartLoadFromStream(stream.AsRandomAccessStream());
        }

        var opacityBrush = compositor.CreateSurfaceBrush(_maskSurface);
        opacityBrush.Stretch = CompositionStretch.UniformToFill;

        var effectFactory = compositor.CreateEffectFactory(effect);
        var compositionBrush = effectFactory.CreateBrush();
        compositionBrush.SetSourceParameter("Source", source);
        compositionBrush.SetSourceParameter("Mask", opacityBrush);
        return (compositionBrush, opacityBrush);
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

    private void StartOffseteAnimation(SpriteVisual visual, TimeSpan duration, TimeSpan delay)
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

    private void StartScaleAnimation(CompositionSurfaceBrush brush, List<(float, float)> keyFrames, TimeSpan duration, TimeSpan delay)
    {
        var offsetAnimation = Compositor.CreateVector2KeyFrameAnimation();
        offsetAnimation.Duration = duration;
        offsetAnimation.DelayTime = delay;
        offsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

        foreach (var item in keyFrames) offsetAnimation.InsertKeyFrame(item.Item1, new Vector2(1, item.Item2 * 2));

        brush.StartAnimation(nameof(CompositionSurfaceBrush.Scale), offsetAnimation);
    }

    private void StopAnimation(SpriteVisual visual)
    {
        if (visual != null)
        {
            visual.StopAnimation(nameof(CompositionSurfaceBrush.Offset));
        }

        if (_storyboard != null)
        {
            _storyboard.Stop();
        }
    }

    private void StopAnimation(CompositionSurfaceBrush compositionSurfaceBrush)
    {
        if (compositionSurfaceBrush != null)
        {
            compositionSurfaceBrush.StopAnimation(nameof(CompositionSurfaceBrush.Scale));
        }
    }
}
