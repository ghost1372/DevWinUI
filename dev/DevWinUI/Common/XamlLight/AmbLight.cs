namespace DevWinUI;

public partial class AmbLight : XamlLight
{
    private static readonly string Id = typeof(AmbLight).FullName;

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color), typeof(Color), typeof(AmbLight), new PropertyMetadata(Colors.White, (s, e) =>
        {
            var self = (AmbLight)s;
            var newColor = (Color)e.NewValue;

            if (self.CompositionLight is AmbientLight ambientLight)
            {
                ambientLight.Color = newColor;
            }
        }));

    public double Intensity
    {
        get => (double)GetValue(IntensityProperty);
        set => SetValue(IntensityProperty, value);
    }
    public static readonly DependencyProperty IntensityProperty = DependencyProperty.Register(
        nameof(Intensity), typeof(double), typeof(AmbLight), new PropertyMetadata(1.0d, (s, e) =>
        {
            var self = (AmbLight)s;
            var newIntensity = (float)e.NewValue;

            if (self.CompositionLight is AmbientLight ambientLight)
            {
                ambientLight.Intensity = newIntensity;
            }
        }));

    protected override void OnConnected(UIElement newElement)
    {
        var compositor = CompositionTarget.GetCompositorForCurrentThread();

        var ambientLight = CreateAmbientLight();
        CompositionLight = ambientLight;

        AddTargetElement(GetId(), newElement);

        AmbientLight CreateAmbientLight()
        {
            var light = compositor.CreateAmbientLight();

            light.Color = Color;
            light.Intensity = Intensity.ToFloat();

            return light;
        }
    }

    protected override void OnDisconnected(UIElement oldElement)
    {
        RemoveTargetElement(GetId(), oldElement);
        CompositionLight.Dispose();
    }

    protected override string GetId() => Id;
}


