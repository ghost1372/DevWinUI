// https://github.com/DinoChan

using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextBackground), Type = typeof(Rectangle))]
public partial class TextGlitchEffect : Control
{
    private string PART_TextBackground = "PART_TextBackground";
    private Rectangle _rectangle;
    private Compositor Compositor => ElementCompositionPreview.GetElementVisual(this).Compositor;
    private SpriteVisual lineVisual;
    private TextToBrushWrapper secondaryWrapper;
    private TextToBrushWrapper primaryWrapper;

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextGlitchEffect)d;
        if (ctl != null)
        {
            ctl.UpdateTextEffect();
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
    private void TextGlitchEffect_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateTextEffect();
    }

    private void UpdateTextEffect()
    {
        if (_rectangle == null)
        {
            return;
        }

        secondaryWrapper = CreateTextToBrushWrapper(SecondaryForeground, SecondaryBackground);
        primaryWrapper = CreateTextToBrushWrapper(PrimaryForeground, PrimaryBackground);

        if (PrimaryTextToBrushWrapper != null)
        {
            primaryWrapper = PrimaryTextToBrushWrapper;
        }

        if (SecondaryTextToBrushWrapper != null)
        {
            secondaryWrapper = SecondaryTextToBrushWrapper;
        }

        if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(primaryWrapper.Text) && string.IsNullOrEmpty(secondaryWrapper.Text))
        {
            return;
        }

        var imageVisual = Compositor.CreateSpriteVisual();
        imageVisual.Brush = CreateBrush(secondaryWrapper.Brush, primaryWrapper.Brush, this.BlendEffect);
        imageVisual.Size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(RenderSize.Height));

        var containerVisual = Compositor.CreateContainerVisual();

        lineVisual = Compositor.CreateSpriteVisual();
        lineVisual.Brush = Compositor.CreateColorBrush(LineColor);
        lineVisual.Size = new Vector2(Convert.ToSingle(RenderSize.Width), Convert.ToSingle(LineHeight));

        containerVisual.Children.InsertAtBottom(imageVisual);
        containerVisual.Children.InsertAtTop(lineVisual);

        ElementCompositionPreview.SetElementChildVisual(_rectangle, containerVisual);

        StopOffsetAnimation(lineVisual);
        StopOffsetAnimation(secondaryWrapper.Brush);
        StopOffsetAnimation(primaryWrapper.Brush);
        StartAnimation();
    }

    private void TextGlitchEffect_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTextEffect();
    }

    private CompositionBrush CreateBrush(CompositionBrush foreground, CompositionBrush background, BlendEffectMode blendEffectMode)
    {
        var effect = new BlendEffect
        {
            Mode = blendEffectMode,
            Foreground = new CompositionEffectSourceParameter("Main"),
            Background = new CompositionEffectSourceParameter("Tint")
        };
        var effectFactory = Compositor.CreateEffectFactory(effect);
        var compositionBrush = effectFactory.CreateBrush();
        compositionBrush.SetSourceParameter("Main", foreground);
        compositionBrush.SetSourceParameter("Tint", background);

        return compositionBrush;
    }

    private void StartAnimation()
    {
        StartOffsetAnimation(secondaryWrapper.Brush, TimeSpan.FromSeconds(0.95), TimeSpan.Zero);
        StartOffsetAnimation(primaryWrapper.Brush, TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(0.2));
        StartOffsetAnimation(lineVisual, TimeSpan.FromSeconds(10), TimeSpan.Zero);
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

        addKey(.9f, 95);
        addKey(.14f, 20);
        addKey(.18f, 105);
        addKey(.22f, 3);
        addKey(.32f, 80);
        addKey(.34f, 30);
        addKey(.4f, 65);
        addKey(.43f, 18);
        addKey(.99f, 75);

        visual.StartAnimation(nameof(CompositionSurfaceBrush.Offset), offsetAnimation);
    }

    private void StartOffsetAnimation(CompositionSurfaceBrush brush, TimeSpan duration, TimeSpan delay)
    {
        var offsetAnimation = Compositor.CreateVector2KeyFrameAnimation();
        offsetAnimation.Duration = duration;
        offsetAnimation.DelayTime = delay;
        offsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

        void addKey(float key, float top, float left)
        {
            offsetAnimation.InsertKeyFrame(key, new Vector2(top * 2.5f, left * 2.5f));
        }

        addKey(.1f, -0.4f, -1.1f);
        addKey(.2f, 0.4f, -0.2f);
        addKey(.3f, 0f, .5f);
        addKey(.4f, -0.3f, -0.7f);
        addKey(.5f, 0, .2f);
        addKey(.6f, 1.8f, 1.2f);
        addKey(.7f, -1f, .1f);
        addKey(.8f, -0.4f, -0.9f);
        addKey(.9f, 0, 1.2f);
        addKey(1, 0, -1.2f);
        brush.StartAnimation(nameof(CompositionSurfaceBrush.Offset), offsetAnimation);
    }
    private void StopOffsetAnimation(SpriteVisual visual)
    {
        if (visual != null)
        {
            visual.StopAnimation(nameof(CompositionSurfaceBrush.Offset));
        }
    }
    private void StopOffsetAnimation(CompositionSurfaceBrush brush)
    {
        if (brush != null)
        {
            brush.StopAnimation(nameof(CompositionSurfaceBrush.Offset));
        }
    }

    public TextToBrushWrapper CreateTextToBrushWrapper(Color fontColor, Brush background)
    {
        var result = new TextToBrushWrapper
        {
            Text = Text,
            FontSize = FontSize,
            Width = Convert.ToSingle(RenderSize.Width),
            Height = Convert.ToSingle(RenderSize.Height),
            FontColor = fontColor,
            Background = background
        };
        return result;
    }
}
