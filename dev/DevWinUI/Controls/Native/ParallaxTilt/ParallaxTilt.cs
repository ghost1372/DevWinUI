using Microsoft.UI.Xaml.Input;

// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public partial class ParallaxTilt : ContentControl
{
    private const string PART_RootGrid = "PART_RootGrid";
    private const string PART_ParallaxTransform = "PART_ParallaxTransform";
    private Grid rootGrid;
    private readonly PlaneProjection planeProjection = new PlaneProjection();
    private CompositeTransform compositeTransform;

    public static readonly DependencyProperty MaxTiltAngleProperty =
        DependencyProperty.Register(nameof(MaxTiltAngle), typeof(double), typeof(ParallaxTilt), new PropertyMetadata(15.0));

    public double MaxTiltAngle
    {
        get => (double)GetValue(MaxTiltAngleProperty);
        set => SetValue(MaxTiltAngleProperty, value);
    }

    public static readonly DependencyProperty ParallaxDepthProperty =
        DependencyProperty.Register(nameof(ParallaxDepth), typeof(double), typeof(ParallaxTilt), new PropertyMetadata(10.0));

    public double ParallaxDepth
    {
        get => (double)GetValue(ParallaxDepthProperty);
        set => SetValue(ParallaxDepthProperty, value);
    }

    private double _targetRotationX = 0;
    private double _targetRotationY = 0;
    private double _targetTranslateX = 0;
    private double _targetTranslateY = 0;

    private double _currentRotationX = 0;
    private double _currentRotationY = 0;
    private double _currentTranslateX = 0;
    private double _currentTranslateY = 0;

    private bool _isPointerInside = false;

    public ParallaxTilt()
    {
        DefaultStyleKey = typeof(ParallaxTilt);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        rootGrid = GetTemplateChild(PART_RootGrid) as Grid;
        compositeTransform = GetTemplateChild(PART_ParallaxTransform) as CompositeTransform;

        rootGrid.Projection = planeProjection;

        CompositionTarget.Rendering -= OnRendering;
        CompositionTarget.Rendering += OnRendering;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;

        PointerEntered -= OnPointerEntered;
        PointerEntered += OnPointerEntered;

        PointerMoved -= OnPointerMoved;
        PointerMoved += OnPointerMoved;

        PointerExited -= OnPointerExited;
        PointerExited += OnPointerExited;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _isPointerInside = true;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isPointerInside || rootGrid == null)
            return;

        var pointerPos = e.GetCurrentPoint(rootGrid).Position;
        var width = rootGrid.ActualWidth;
        var height = rootGrid.ActualHeight;

        if (width == 0 || height == 0) return;

        double normalizedX = (pointerPos.X - width / 2) / (width / 2);
        double normalizedY = (pointerPos.Y - height / 2) / (height / 2);

        _targetRotationY = -normalizedX * MaxTiltAngle;
        _targetRotationX = normalizedY * MaxTiltAngle;

        _targetTranslateX = -normalizedX * ParallaxDepth;
        _targetTranslateY = -normalizedY * ParallaxDepth;
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _isPointerInside = false;

        _targetRotationX = 0;
        _targetRotationY = 0;
        _targetTranslateX = 0;
        _targetTranslateY = 0;
    }

    private void OnRendering(object? sender, object e)
    {        
        const double lerpFactor = 0.15;

        if (Math.Abs(_targetRotationX - _currentRotationX) > 0.01 ||
            Math.Abs(_targetRotationY - _currentRotationY) > 0.01)
        {
            _currentRotationX += (_targetRotationX - _currentRotationX) * lerpFactor;
            _currentRotationY += (_targetRotationY - _currentRotationY) * lerpFactor;
            _currentTranslateX += (_targetTranslateX - _currentTranslateX) * lerpFactor;
            _currentTranslateY += (_targetTranslateY - _currentTranslateY) * lerpFactor;

            planeProjection?.RotationX = _currentRotationX;
            planeProjection?.RotationY = _currentRotationY;
            compositeTransform?.TranslateX = _currentTranslateX;
            compositeTransform?.TranslateY = _currentTranslateY;
        }
    }
}
