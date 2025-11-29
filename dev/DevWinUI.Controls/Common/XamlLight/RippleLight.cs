using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class RippleLight : XamlLight
{
    private static readonly string Id = typeof(RippleLight).FullName;

    private Compositor _compositor;

    private const float OffsetZRatio = 2.0f;
    private float _rippleOffsetZ;
    private Vector3KeyFrameAnimation _lightRippleOffsetAnimation;

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(RippleLight), new PropertyMetadata(Colors.White, (s, e) =>
        {
            var self = (RippleLight)s;
            var newColor = (Color)e.NewValue;

            if (self.CompositionLight is SpotLight spotLight)
            {
                spotLight.InnerConeColor = newColor;
                spotLight.OuterConeColor = newColor;
            }
        }));

    protected override void OnConnected(UIElement newElement)
    {
        _compositor = CompositionTarget.GetCompositorForCurrentThread();

        var spotLight = CreateSpotLight();
        CompositionLight = spotLight;

        _lightRippleOffsetAnimation = CreateLightRippleOffsetAnimation();

        SubscribeToPointerEvents();

        AddTargetElement(GetId(), newElement);

        SpotLight CreateSpotLight()
        {
            var light = _compositor.CreateSpotLight();

            light.InnerConeColor = light.OuterConeColor = Color;
            light.InnerConeAngleInDegrees = 90.0f;
            light.OuterConeAngleInDegrees = 0.0f;
            //light.LinearAttenuation = 0.2f;
            //light.QuadraticAttenuation = 0.1f;
            _rippleOffsetZ = CalculateRippleOffsetZOnDesizedSize(newElement);
            light.Offset = new Vector3(0.0f, 0.0f, _rippleOffsetZ);

            return light;
        }

        void SubscribeToPointerEvents()
        {
            if (newElement is FrameworkElement element)
            {
                element.SizeChanged += OnElementSizeChanged;
            }
            newElement.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
        }
    }

    protected override void OnDisconnected(UIElement oldElement)
    {
        UnsubscribeFromPointerEvents();

        RemoveTargetElement(GetId(), oldElement);
        CompositionLight.Dispose();

        _lightRippleOffsetAnimation.Dispose();

        void UnsubscribeFromPointerEvents()
        {
            if (oldElement is FrameworkElement element)
            {
                element.SizeChanged -= OnElementSizeChanged;
            }
            oldElement.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
        }
    }

    protected override string GetId() => Id;

    private void OnElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.PreviousSize.Equals(e.NewSize)) return;

        _rippleOffsetZ = CalculateRippleOffsetZOnRenderSize((FrameworkElement)sender);
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e) =>
        StartLightRippleOffsetAnimation(e.GetCurrentPoint((UIElement)sender).Position.ToVector2());

    private Vector3KeyFrameAnimation CreateLightRippleOffsetAnimation()
    {
        var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(1200);

        return offsetAnimation;
    }

    private void StartLightRippleOffsetAnimation(Vector2 position)
    {
        var startingPoisition = new Vector3(position, 0.0f);
        _lightRippleOffsetAnimation?.InsertKeyFrame(0.0f, startingPoisition);
        _lightRippleOffsetAnimation?.InsertKeyFrame(1.0f, new Vector3(position.X, position.Y, _rippleOffsetZ));

        CompositionLight?.StartAnimation("Offset", _lightRippleOffsetAnimation);
    }

    private float CalculateRippleOffsetZOnDesizedSize(UIElement element)
    {
        var desiredSize = element.DesiredSize;
        return (float)Math.Max(desiredSize.Width, desiredSize.Height) * OffsetZRatio;
    }

    private float CalculateRippleOffsetZOnRenderSize(UIElement element)
    {
        var desiredSize = element.RenderSize.ToVector2();
        return Math.Max(desiredSize.X, desiredSize.Y) * OffsetZRatio;
    }
}
