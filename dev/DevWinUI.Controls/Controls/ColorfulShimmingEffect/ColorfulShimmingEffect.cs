using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = PART_Root, Type = (typeof(StackPanel)))]
[ContentProperty(Name = nameof(Content))]
public partial class ColorfulShimmingEffect : ContentControl
{
    private const string PART_Root = "PART_Root";
    private StackPanel _rootPanel;
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
    }

    private void ColorfulShimmingEffect_Loaded(object sender, RoutedEventArgs e)
    {
        ShowShimmingEffect();
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
        var rootVisual = _rootPanel.GetVisualInternal();

        foreach (var item in ColorfulShimmingEffectItems)
        {
            var pointLight = CreatePointAndStartAnimation(item.Color, item.DelayTimeSpan, item.DurationTimeSpan, item.Z);

            pointLight.Targets.Add(rootVisual);
        }
    }

    public PointLight CreatePointAndStartAnimation(Color color, TimeSpan delay, TimeSpan duration, double z)
    {
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
