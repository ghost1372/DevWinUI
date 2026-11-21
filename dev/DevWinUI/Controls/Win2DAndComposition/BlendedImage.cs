namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ImageGrid), Type = typeof(Grid))]
public partial class BlendedImage : Control
{
    private const string PART_ImageGrid = "PART_ImageGrid";
    private Grid? grid;

    private Compositor? _compositor;
    private ICompositionGenerator? _generator;
    private CanvasGeometry? _leftGeometry;
    private CanvasGeometry? _rightGeometry;
    private float _topOffset;
    private IGaussianMaskSurface? _leftGaussianSurface;
    private IGaussianMaskSurface? _rightGaussianSurface;
    private float _currentWidth;
    private float _currentHeight;
    private int _rebuildVersion;

    public double BlurRadius
    {
        get { return (double)GetValue(BlurRadiusProperty); }
        set { SetValue(BlurRadiusProperty, value); }
    }

    public static readonly DependencyProperty BlurRadiusProperty =
        DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(BlendedImage), new PropertyMetadata(100.0d, OnPropertyChanged));

    public double SplitValue
    {
        get { return (double)GetValue(SplitValueProperty); }
        set { SetValue(SplitValueProperty, value); }
    }

    public static readonly DependencyProperty SplitValueProperty =
        DependencyProperty.Register(nameof(SplitValue), typeof(double), typeof(BlendedImage), new PropertyMetadata(50.0d, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BlendedImage)d;
        if (ctl != null)
        {
            ctl.Redraw();
        }
    }

    public object? PrimaryImageSource
    {
        get { return GetValue(PrimaryImageSourceProperty); }
        set { SetValue(PrimaryImageSourceProperty, value); }
    }

    public static readonly DependencyProperty PrimaryImageSourceProperty =
        DependencyProperty.Register(nameof(PrimaryImageSource), typeof(object), typeof(BlendedImage), new PropertyMetadata(null, OnSourceChanged));

    public object? SecondaryImageSource
    {
        get { return GetValue(SecondaryImageSourceProperty); }
        set { SetValue(SecondaryImageSourceProperty, value); }
    }

    public static readonly DependencyProperty SecondaryImageSourceProperty =
        DependencyProperty.Register(nameof(SecondaryImageSource), typeof(object), typeof(BlendedImage), new PropertyMetadata(null, OnSourceChanged));

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BlendedImage)d;
        if (ctl != null)
        {
            ctl.RebuildSurfaces();
        }
    }

    public BlendedImage()
    {
        DefaultStyleKey = typeof(BlendedImage);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        grid = GetTemplateChild(PART_ImageGrid) as Grid;

        _compositor = CompositionTarget.GetCompositorForCurrentThread();
        _generator = _compositor.CreateCompositionGenerator();

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0)
            return;

        _currentWidth = (float)ActualWidth;
        _currentHeight = (float)ActualHeight;

        RebuildSurfaces();
    }

    private float GetSplitValue()
    {
        var value = (float)SplitValue;
        return float.IsNaN(value) ? 0f : Math.Clamp(value, 0f, 100f);
    }

    private float GetBlurRadius()
    {
        var value = (float)BlurRadius;
        return float.IsNaN(value) ? 0f : Math.Max(value, 0f);
    }

    private async void RebuildSurfaces()
    {
        var host = grid;
        var compositor = _compositor;
        var generator = _generator;

        if (host == null || compositor == null || generator == null || PrimaryImageSource == null || SecondaryImageSource == null || _currentWidth <= 0 || _currentHeight <= 0)
            return;

        var primaryUri = GeneralHelper.GetUriFromObjectSource(PrimaryImageSource);
        var secondaryUri = GeneralHelper.GetUriFromObjectSource(SecondaryImageSource);
        if (primaryUri == null || secondaryUri == null)
            return;

        var version = ++_rebuildVersion;
        var size = new Vector2(_currentWidth, _currentHeight);

        var leftImageTask = generator.CreateImageSurfaceAsync(primaryUri, size.ToSize(), ImageSurfaceOptions.Default);
        var rightImageTask = generator.CreateImageSurfaceAsync(secondaryUri, size.ToSize(), ImageSurfaceOptions.Default);

        var leftImageSurface = await leftImageTask;
        var rightImageSurface = await rightImageTask;

        if (version != _rebuildVersion)
            return;

        var splitValue = GetSplitValue();
        var blurRadius = GetBlurRadius();
        _topOffset = -_currentHeight / 2f;

        var leftImageVisual = compositor.CreateSpriteVisual();
        leftImageVisual.Size = size;

        _leftGeometry?.Dispose();
        _leftGeometry = CanvasGeometry.CreateRectangle(
            generator.Device,
            new Rect(0f, 0f, 2f * (_currentWidth * splitValue) / 100f, 2f * _currentHeight)
        );

        var leftOffset = -(_currentWidth * splitValue) / 100f;

        var leftMaskedBrush = compositor.CreateMaskBrush();
        leftMaskedBrush.Source = compositor.CreateSurfaceBrush(leftImageSurface);
        _leftGaussianSurface = generator.CreateGaussianMaskSurface(size.ToSize(), _leftGeometry, new Vector2(leftOffset, _topOffset), blurRadius);
        leftMaskedBrush.Mask = compositor.CreateSurfaceBrush(_leftGaussianSurface);
        leftImageVisual.Brush = leftMaskedBrush;

        var rightImageVisual = compositor.CreateSpriteVisual();
        rightImageVisual.Size = size;

        _rightGeometry?.Dispose();
        _rightGeometry = CanvasGeometry.CreateRectangle(
            generator.Device,
            new Rect(0, 0, 2f * (_currentWidth * (100f - splitValue)) / 100f, 2f * _currentHeight)
        );

        var rightMaskedBrush = compositor.CreateMaskBrush();
        rightMaskedBrush.Source = compositor.CreateSurfaceBrush(rightImageSurface);
        _rightGaussianSurface = generator.CreateGaussianMaskSurface(size.ToSize(), _rightGeometry, new Vector2(-leftOffset, _topOffset), blurRadius);
        rightMaskedBrush.Mask = compositor.CreateSurfaceBrush(_rightGaussianSurface);
        rightImageVisual.Brush = rightMaskedBrush;

        var container = compositor.CreateContainerVisual();
        container.Size = size;
        container.Children.InsertAtTop(leftImageVisual);
        container.Children.InsertAtTop(rightImageVisual);

        ElementCompositionPreview.SetElementChildVisual(host, container);
    }

    private void Redraw()
    {
        var generator = _generator;
        if (generator == null || _leftGeometry == null || _rightGeometry == null || _leftGaussianSurface == null || _rightGaussianSurface == null)
        {
            return;
        }

        var splitValue = GetSplitValue();
        var blurRadius = GetBlurRadius();
        var leftOffset = -(_currentWidth * splitValue) / 100f;

        _leftGeometry.Dispose();
        _leftGeometry = CanvasGeometry.CreateRectangle(generator.Device, new Rect(0f, 0f, 2f * (_currentWidth * splitValue) / 100f, 2f * _currentHeight));
        _leftGaussianSurface.Redraw(_leftGeometry, new Vector2(leftOffset, _topOffset), blurRadius);
        _rightGeometry.Dispose();
        _rightGeometry = CanvasGeometry.CreateRectangle(generator.Device, new Rect(0, 0, 2f * (_currentWidth * (100f - splitValue)) / 100, 2f * _currentHeight));
        _rightGaussianSurface.Redraw(_rightGeometry, new Vector2(-leftOffset, _topOffset), blurRadius);
    }
}
