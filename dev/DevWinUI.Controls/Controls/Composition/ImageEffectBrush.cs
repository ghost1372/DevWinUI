using Windows.Graphics.Effects;

namespace DevWinUI;
public partial class ImageEffectBrush : XamlCompositionBrushBase
{
    private LoadedImageSurface _surface;
    private CompositionSurfaceBrush _surfaceBrush;

    public string ImageUri
    {
        get { return (string)GetValue(ImageUriStringProperty); }
        set { SetValue(ImageUriStringProperty, value); }
    }

    public static readonly DependencyProperty ImageUriStringProperty =
        DependencyProperty.Register(nameof(ImageUri), typeof(string), typeof(ImageEffectBrush),new PropertyMetadata(string.Empty, OnImageUriChanged));

    private static void OnImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var brush = (ImageEffectBrush)d;
        // Unbox and update surface if CompositionBrush exists     
        if (brush._surfaceBrush != null)
        {
            var newSurface = LoadedImageSurface.StartLoadFromUri(new Uri((string)e.NewValue));
            brush._surface = newSurface;
            brush._surfaceBrush.Surface = newSurface;
        }
    }

    protected override void OnConnected()
    {
        // return if Uri String is null or empty
        if (String.IsNullOrEmpty(ImageUri))
        {
            return;
        }

        Compositor compositor = CompositionTarget.GetCompositorForCurrentThread();

        // Use LoadedImageSurface API to get ICompositionSurface from image uri provided
        _surface = LoadedImageSurface.StartLoadFromUri(new Uri(ImageUri));

        // Load Surface onto SurfaceBrush
        _surfaceBrush = compositor.CreateSurfaceBrush(_surface);
        _surfaceBrush.Stretch = CompositionStretch.UniformToFill;

        // CompositionCapabilities: Are Tint+Temperature and Saturation supported?
        var capabilities = new CompositionCapabilities();
        bool usingFallback = !capabilities.AreEffectsSupported();
        if (usingFallback)
        {
            // If Effects are not supported, Fallback to image without effects
            CompositionBrush = _surfaceBrush;
            return;
        }

        // Define Effect graph
        IGraphicsEffect graphicsEffect = new SaturationEffect
        {
            Name = "Saturation",
            Saturation = 0.3f,
            Source = new TemperatureAndTintEffect
            {
                Name = "TempAndTint",
                Temperature = 0,
                Source = new CompositionEffectSourceParameter("Surface"),
            }
        };

        // Create EffectFactory and EffectBrush 
        CompositionEffectFactory effectFactory = compositor.CreateEffectFactory(graphicsEffect, new[] { "TempAndTint.Temperature" });
        CompositionEffectBrush effectBrush = effectFactory.CreateBrush();
        effectBrush.SetSourceParameter("Surface", _surfaceBrush);

        // Set EffectBrush to paint Xaml UIElement
        CompositionBrush = effectBrush;

        // Trivial looping animation to demonstrate animated effect
        ScalarKeyFrameAnimation tempAnim = compositor.CreateScalarKeyFrameAnimation();
        tempAnim.InsertKeyFrame(0, 0);
        tempAnim.InsertKeyFrame(0.5f, 1f);
        tempAnim.InsertKeyFrame(1, 0);
        tempAnim.Duration = TimeSpan.FromSeconds(5);
        tempAnim.IterationBehavior = Microsoft.UI.Composition.AnimationIterationBehavior.Forever;
        effectBrush.Properties.StartAnimation("TempAndTint.Temperature", tempAnim);
    }

    protected override void OnDisconnected()
    {
        // Dispose Surface and CompositionBrushes if XamlCompBrushBase is removed from tree
        if (_surface != null)
        {
            _surface.Dispose();
            _surface = null;
        }
        if (CompositionBrush != null)
        {
            CompositionBrush.Dispose();
            CompositionBrush = null;
        }
    }
}
