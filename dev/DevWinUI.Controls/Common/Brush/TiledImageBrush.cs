namespace DevWinUI;

public partial class TiledImageBrush : XamlCompositionBrushBase
{
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(TiledImageBrush), new PropertyMetadata(default(ImageSource), OnImageSourceUriChanged));

    private BorderEffect _borderEffect;
    private CompositionEffectBrush _borderEffectBrush;
    private CompositionEffectFactory _borderEffectFactory;
    private LoadedImageSurface _surface;
    private CompositionSurfaceBrush _surfaceBrush;

    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private Compositor Compositor => CompositionTarget.GetCompositorForCurrentThread();

    protected override void OnConnected()
    {
        base.OnConnected();

        if (CompositionBrush == null)
        {
            _surfaceBrush = Compositor.CreateSurfaceBrush();
            _surfaceBrush.Stretch = CompositionStretch.None;

            UpdateSurface();

            _borderEffect = new BorderEffect()
            {
                Source = new CompositionEffectSourceParameter("source"),
                ExtendX = Microsoft.Graphics.Canvas.CanvasEdgeBehavior.Wrap,
                ExtendY = Microsoft.Graphics.Canvas.CanvasEdgeBehavior.Wrap
            };

            _borderEffectFactory = Compositor.CreateEffectFactory(_borderEffect);
            _borderEffectBrush = _borderEffectFactory.CreateBrush();
            _borderEffectBrush.SetSourceParameter("source", _surfaceBrush);
            CompositionBrush = _borderEffectBrush;
        }
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();

        if (CompositionBrush != null)
        {
            CompositionBrush.Dispose();
            CompositionBrush = null;
        }

        if (_borderEffectBrush != null)
        {
            _borderEffectBrush.Dispose();
            _borderEffectBrush = null;
        }

        if (_borderEffectFactory != null)
        {
            _borderEffectFactory.Dispose();
            _borderEffectFactory = null;
        }

        if (_borderEffect != null)
        {
            _borderEffect.Dispose();
            _borderEffect = null;
        }

        if (_surface != null)
        {
            _surface.Dispose();
            _surface = null;
        }

        if (_surfaceBrush != null)
        {
            _surfaceBrush.Dispose();
            _surfaceBrush = null;
        }
    }

    protected virtual void OnImageSourceUriChanged(ImageSource oldValue, ImageSource newValue)
    {
        UpdateSurface();
    }

    private static void OnImageSourceUriChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (ImageSource)args.OldValue;
        var newValue = (ImageSource)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TiledImageBrush;
        target?.OnImageSourceUriChanged(oldValue, newValue);
    }

    private void UpdateSurface()
    {
        if (Source != null && _surfaceBrush != null)
        {
            var uri = (Source as BitmapImage)?.UriSource ?? new Uri("ms-appx:///");
            _surface = LoadedImageSurface.StartLoadFromUri(uri);
            _surfaceBrush.Surface = _surface;
        }
    }
}
