namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ImageGrid), Type = typeof(Grid))]
public partial class BlendedImage : Control
{
    private const string PART_ImageGrid = "PART_ImageGrid";
    private Grid grid;

    private Compositor _compositor;
    private ICompositionGenerator _generator;
    private CanvasGeometry _leftGeometry;
    private CanvasGeometry _rightGeometry;
    private float _topOffset;
    private IGaussianMaskSurface _leftGaussianSurface;
    private IGaussianMaskSurface _rightGaussianSurface;
    private float _currentWidth;
    private float _currentHeight;

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

    public object PrimaryImageSource
    {
        get { return (object)GetValue(PrimaryImageSourceProperty); }
        set { SetValue(PrimaryImageSourceProperty, value); }
    }

    public static readonly DependencyProperty PrimaryImageSourceProperty =
        DependencyProperty.Register(nameof(PrimaryImageSource), typeof(object), typeof(BlendedImage), new PropertyMetadata(null, OnSourceChanged));

    public object SecondaryImageSource
    {
        get { return (object)GetValue(SecondaryImageSourceProperty); }
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
    private async void RebuildSurfaces()
    {
        if (grid == null || _generator == null || PrimaryImageSource == null || SecondaryImageSource == null)
            return;

        var size = new Vector2(_currentWidth, _currentHeight);
        _topOffset = -_currentHeight / 2f;

        // Create visuals again
        var leftImageVisual = _compositor.CreateSpriteVisual();
        leftImageVisual.Size = size;

        var leftImageSurface =
            await _generator.CreateImageSurfaceAsync(
                GeneralHelper.GetUriFromObjectSource(PrimaryImageSource),
                size.ToSize(),
                ImageSurfaceOptions.Default
            );

        _leftGeometry = CanvasGeometry.CreateRectangle(
            _generator.Device,
            new Rect(0f, 0f, 2f * (_currentWidth * (float)SplitValue) / 100f, 2f * _currentHeight)
        );

        var leftOffset = -(_currentWidth * (float)SplitValue) / 100f;

        var leftMaskedBrush = _compositor.CreateMaskBrush();
        leftMaskedBrush.Source = _compositor.CreateSurfaceBrush(leftImageSurface);
        _leftGaussianSurface = _generator.CreateGaussianMaskSurface(size.ToSize(), _leftGeometry, new Vector2(leftOffset, _topOffset), (float)BlurRadius);
        leftMaskedBrush.Mask = _compositor.CreateSurfaceBrush(_leftGaussianSurface);
        leftImageVisual.Brush = leftMaskedBrush;

        // Right side
        var rightImageVisual = _compositor.CreateSpriteVisual();
        rightImageVisual.Size = size;

        var rightImageSurface =
            await _generator.CreateImageSurfaceAsync(
                GeneralHelper.GetUriFromObjectSource(SecondaryImageSource),
                size.ToSize(),
                ImageSurfaceOptions.Default
            );

        _rightGeometry = CanvasGeometry.CreateRectangle(
            _generator.Device,
            new Rect(0, 0, 2f * (_currentWidth * (100f - (float)SplitValue)) / 100f, 2f * _currentHeight)
        );

        var rightMaskedBrush = _compositor.CreateMaskBrush();
        rightMaskedBrush.Source = _compositor.CreateSurfaceBrush(rightImageSurface);
        _rightGaussianSurface = _generator.CreateGaussianMaskSurface(size.ToSize(), _rightGeometry, new Vector2(-leftOffset, _topOffset), (float)BlurRadius);
        rightMaskedBrush.Mask = _compositor.CreateSurfaceBrush(_rightGaussianSurface);
        rightImageVisual.Brush = rightMaskedBrush;

        // Replace child visual
        var container = _compositor.CreateContainerVisual();
        container.Size = size;
        container.Children.InsertAtTop(leftImageVisual);
        container.Children.InsertAtTop(rightImageVisual);

        ElementCompositionPreview.SetElementChildVisual(grid, container);
    }

    private void Redraw()
    {
        if (_leftGeometry == null || _rightGeometry == null)
        {
            return;
        }

        var leftOffset = -(_currentWidth * (float)SplitValue) / 100f;
        _leftGeometry = CanvasGeometry.CreateRectangle(_generator.Device, new Rect(0f, 0f, 2f * (_currentWidth * (float)SplitValue) / 100f, 2f * _currentHeight));
        _leftGaussianSurface.Redraw(_leftGeometry, new Vector2(leftOffset, _topOffset), (float)BlurRadius);
        _rightGeometry = CanvasGeometry.CreateRectangle(_generator.Device, new Rect(0, 0, 2f * (_currentWidth * (100f - (float)SplitValue)) / 100, 2f * _currentHeight));
        _rightGaussianSurface.Redraw(_rightGeometry, new Vector2(-leftOffset, _topOffset), (float)BlurRadius);
    }
}

