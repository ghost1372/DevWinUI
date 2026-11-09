using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = PART_Root, Type = (typeof(StackPanel)))]
[ContentProperty(Name = nameof(Content))]
public partial class ColorfulShimmingEffect : ContentControl
{
    private const string PART_Root = "PART_Root";
    private StackPanel _rootPanel;
    private readonly List<PointLight> _lights = new();
    public List<ColorfulShimmingEffectItem> ColorfulShimmingEffectItems
    {
        get { return (List<ColorfulShimmingEffectItem>)GetValue(ColorfulShimmingEffectItemsProperty); }
        set { SetValue(ColorfulShimmingEffectItemsProperty, value); }
    }

    public static readonly DependencyProperty ColorfulShimmingEffectItemsProperty =
        DependencyProperty.Register(nameof(ColorfulShimmingEffectItems), typeof(List<ColorfulShimmingEffectItem>), typeof(ColorfulShimmingEffect), new PropertyMetadata(null, OnPointsChanged));

    public ColorfulShimmingEffect()
    {
        this.DefaultStyleKey = typeof(ColorfulShimmingEffect);

        if (ColorfulShimmingEffectItems == null)
        {
            ColorfulShimmingEffectItems = new List<ColorfulShimmingEffectItem>();
        }
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _rootPanel = GetTemplateChild(PART_Root) as StackPanel;
        if (ColorfulShimmingEffectItems == null || ColorfulShimmingEffectItems.Count == 0)
        {
            ColorfulShimmingEffectItems = new List<ColorfulShimmingEffectItem>
            {
                new ColorfulShimmingEffectItem
                {
                    Color = Color.FromArgb(255, 0, 27, 171),
                    DelayTimeSpan = TimeSpan.Zero,
                    DurationTimeSpan = TimeSpan.FromSeconds(10),
                    Z = Convert.ToSingle(75.0f)
                },
                new ColorfulShimmingEffectItem
                {
                    Color = Color.FromArgb(255, 217, 17, 83),
                    DelayTimeSpan = TimeSpan.FromSeconds(0.25),
                    DurationTimeSpan = TimeSpan.FromSeconds(10),
                    Z = Convert.ToSingle(75.0f)
                }
            };
        }
        Loaded -= ColorfulShimmingEffect_Loaded;
        Loaded += ColorfulShimmingEffect_Loaded;

        Unloaded -= ColorfulShimmingEffect_Unloaded;
        Unloaded += ColorfulShimmingEffect_Unloaded;

        SizeChanged -= ColorfulShimmingEffect_SizeChanged;
        SizeChanged += ColorfulShimmingEffect_SizeChanged;
    }

    private void ColorfulShimmingEffect_Loaded(object sender, RoutedEventArgs e)
    {
        ShowShimmingEffect();
    }

    private void ColorfulShimmingEffect_Unloaded(object sender, RoutedEventArgs e)
    {
        // Stop the endless animations while the control is not in the tree.
        ClearLights();
    }

    private void ColorfulShimmingEffect_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // The light offsets depend on the size (and the effect can't start while the width is 0).
        ShowShimmingEffect();
    }

    private void ClearLights()
    {
        foreach (var light in _lights)
        {
            light.Targets.RemoveAll();
            light.Dispose();
        }
        _lights.Clear();
    }

    private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorfulShimmingEffect)d;
        if (ctl != null)
        {
            ctl.ShowShimmingEffect();
        }
    }

    public void ShowShimmingEffect()
    {
        if (_rootPanel == null || RenderSize.Width == 0 || ColorfulShimmingEffectItems == null)
        {
            return;
        }
        // Remove the lights of a previous call, otherwise they stack up and keep animating.
        ClearLights();

        var rootVisual = _rootPanel.GetVisualInternal();

        foreach (var item in ColorfulShimmingEffectItems)
        {
            var pointLight = CreatePointAndStartAnimation(item.Color, item.DelayTimeSpan, item.DurationTimeSpan, item.Z);
            if (pointLight == null)
            {
                continue;
            }

            pointLight.Targets.Add(rootVisual);
            _lights.Add(pointLight);
        }
    }

    public PointLight CreatePointAndStartAnimation(Color color, TimeSpan delay, TimeSpan duration, double z)
    {
        if (_rootPanel == null)
        {
            return null;
        }

        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        var rootVisual = _rootPanel.GetVisualInternal();
        var pointLight = compositor.CreatePointLight();

        pointLight.Color = color;
        pointLight.CoordinateSpace = rootVisual;
        pointLight.Offset = new Vector3(-Convert.ToSingle(RenderSize.Width) * 4, Convert.ToSingle(RenderSize.Height), Convert.ToSingle(z));

        var offsetAnimation = compositor.CreateScalarKeyFrameAnimation();
        offsetAnimation.InsertKeyFrame(1.0f, Convert.ToSingle(RenderSize.Width) * 5, compositor.CreateLinearEasingFunction());
        offsetAnimation.Duration = duration;
        offsetAnimation.DelayTime = delay;
        offsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

        pointLight.StartAnimation("Offset.X", offsetAnimation);
        return pointLight;
    }
}
