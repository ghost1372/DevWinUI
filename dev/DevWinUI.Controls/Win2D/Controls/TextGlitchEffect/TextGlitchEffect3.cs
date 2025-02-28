// https://github.com/DinoChan

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextBackground), Type = typeof(Rectangle))]
public partial class TextGlitchEffect3 : Control
{
    private string PART_TextBackground = "PART_TextBackground";
    private Rectangle _rectangle;
    private Compositor Compositor => ElementCompositionPreview.GetElementVisual(this).Compositor;
    private TextToBrushWrapper primaryWrapper;
    private TextToBrushWrapper secondaryWrapper;
    private TextToBrushWrapper tertiaryWrapper;
    private Storyboard _storyboard;
    private static async void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextGlitchEffect3)d;
        if (ctl != null)
        {
            await ctl.UpdateTextEffectAsync();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _rectangle = GetTemplateChild(PART_TextBackground) as Rectangle;
        if (_rectangle != null)
        {
            Loaded -= TextGlitchEffect_Loaded;
            Loaded += TextGlitchEffect_Loaded;
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
        await UpdateTextEffectAsync();
    }

    private async Task UpdateTextEffectAsync()
    {
        if (_rectangle == null)
        {
            return;
        }

        primaryWrapper = CreateTextToBrushWrapper(PrimaryForeground, PrimaryBackground, 3, PrimaryShadowColor);
        secondaryWrapper = CreateTextToBrushWrapper(SecondaryForeground, SecondaryBackground, -3, SecondaryShadowColor);
        secondaryWrapper.Brush.Offset = new Vector2(-7f, 0);

        tertiaryWrapper = CreateTextToBrushWrapper(TertiaryForeground, TertiaryBackground, 0, TertiaryShadowColor);


        if (PrimaryTextToBrushWrapper != null)
        {
            primaryWrapper = PrimaryTextToBrushWrapper;
        }

        if (SecondaryTextToBrushWrapper != null)
        {
            secondaryWrapper = SecondaryTextToBrushWrapper;
        }

        if (TertiaryTextToBrushWrapper != null)
        {
            tertiaryWrapper = TertiaryTextToBrushWrapper;
        }

        if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(primaryWrapper.Text) && string.IsNullOrEmpty(secondaryWrapper.Text) && string.IsNullOrEmpty(tertiaryWrapper.Text))
        {
            return;
        }

        var (redBrush, redMaskBrush) = CreateMaskedBrush(primaryWrapper.Brush);
        var (blueBrush, blueMaskBrush) = CreateMaskedBrush(secondaryWrapper.Brush);

        var containerVisual = Compositor.CreateContainerVisual();
        var foregroundVisual = Compositor.CreateSpriteVisual();

        foregroundVisual.Brush = CreateBrush(blueBrush, redBrush, BlendEffect);
        foregroundVisual.Size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(RenderSize.Height));
        containerVisual.Children.InsertAtBottom(foregroundVisual);

        tertiaryWrapper = CreateTextToBrushWrapper(TertiaryForeground, TertiaryBackground, 0, TertiaryShadowColor);

        var textVisual = Compositor.CreateSpriteVisual();
        textVisual.Brush = tertiaryWrapper.Brush;
        textVisual.Size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(RenderSize.Height));
        containerVisual.Children.InsertAtBottom(textVisual);

        var lineVisual = Compositor.CreateSpriteVisual();
        lineVisual.Brush = Compositor.CreateColorBrush(LineColor);
        lineVisual.Size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(LineHeight));
        containerVisual.Children.InsertAtTop(lineVisual);
        ElementCompositionPreview.SetElementChildVisual(_rectangle, containerVisual);

        StopAnimation(lineVisual);
        StopAnimation(redMaskBrush);
        StopAnimation(blueMaskBrush);

        _storyboard = StartHeightAnimation(secondaryWrapper, new List<(double, double)> { (18, 110), (20, 112.5), (25, 110) }, StoryBoardAnimationDuration, TimeSpan.Zero);
        StartOffseteAnimation(lineVisual, OffsetAnimationDuration, TimeSpan.Zero);
        StartScaleAnimation(redMaskBrush, new List<(float, float)> { (0, 0.01f), (.20f, .73f), (.60f, .14f), (1, .95f) }, PrimaryScaleAnimationDuration, TimeSpan.Zero);
        StartScaleAnimation(blueMaskBrush, new List<(float, float)> { (0, 1), (.20f, 1), (.35f, .27f), (.50f, .91f), (.60f, .45f), (.70f, .77f), (.80f, .5f), (1, 0) }, SecondaryScaleAnimationDuration, TimeSpan.Zero);
        var index = 0;
        var words = primaryWrapper.Text.Split(Delimiter);
        while (true)
        {
            primaryWrapper.Text = words[index % words.Length];
            secondaryWrapper.Text = words[index % words.Length];
            tertiaryWrapper.Text = words[index % words.Length];
            await Task.Delay(Delay);
            index++;
        }
    }

    private async void TextGlitchEffect_Loaded(object sender, RoutedEventArgs e)
    {
        await UpdateTextEffectAsync();
    }

    public TextToBrushWrapper CreateTextToBrushWrapper(Color fontColor, Brush background, double shadowOffsetX, Color shadowColor)
    {
        var result = new TextToBrushWrapper
        {
            Text = Text,
            FontSize = FontSize,
            Width = Convert.ToSingle(RenderSize.Width),
            Height = Convert.ToSingle(RenderSize.Height),
            FontColor = fontColor,
            Background = background,
            ShadowBlurAmount = 0,
            ShadowOffsetX = shadowOffsetX,
            ShadowColor = shadowColor,
            FontFamily = new FontFamily("Arial, Helvetica"),
            FontWeight = FontWeights.Thin
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

        var opacityMaskSurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///Assets/mask.Png"));
        var opacityBrush = Compositor.CreateSurfaceBrush(opacityMaskSurface);
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
