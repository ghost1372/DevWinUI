using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = PART_MagnifierHost, Type = typeof(Border))]
public partial class Magnifier : Control
{
    private const string PART_MagnifierHost = "PART_MagnifierHost";
    private CompositionVisualSurface _visualSurface;
    private SpriteVisual _magnifierVisual;
    private Compositor _compositor;

    private FrameworkElement _target;
    private Border _magnifierHost;

    private TranslateTransform _translateTransform;
    private Point _lastPointerPosition;

    public static Magnifier? GetInstance(DependencyObject obj) => (Magnifier?)obj.GetValue(InstanceProperty);
    public static void SetInstance(DependencyObject obj, Magnifier? value) => obj.SetValue(InstanceProperty, value);

    public static readonly DependencyProperty InstanceProperty =
        DependencyProperty.RegisterAttached("Instance", typeof(Magnifier), typeof(Magnifier), new PropertyMetadata(null, OnInstanceChanged));

    private static void OnInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement target && e.NewValue is Magnifier magnifier)
        {
            magnifier.AttachToTarget(target);
        }
    }

    public int SourceSize
    {
        get => (int)GetValue(SourceSizeProperty);
        set => SetValue(SourceSizeProperty, value);
    }

    public static readonly DependencyProperty SourceSizeProperty =
        DependencyProperty.Register(nameof(SourceSize), typeof(int), typeof(Magnifier), new PropertyMetadata(60, OnSourceSizeChanged));

    private static void OnSourceSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Magnifier)d;
        ctl?.Initialize();
    }

    public MagnifierPosition Position
    {
        get => (MagnifierPosition)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register(nameof(Position), typeof(MagnifierPosition), typeof(Magnifier), new PropertyMetadata(MagnifierPosition.Right));

    public double HorizontalOffset
    {
        get => (double)GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    public static readonly DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(Magnifier), new PropertyMetadata(0.0));

    public double VerticalOffset
    {
        get => (double)GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    public static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(Magnifier), new PropertyMetadata(0.0));

    public Magnifier()
    {
        DefaultStyleKey = typeof(Magnifier);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;

        _translateTransform = new TranslateTransform();
        this.RenderTransform = _translateTransform;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _magnifierHost = GetTemplateChild(PART_MagnifierHost) as Border;
        Initialize();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Initialize();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateVisualSize();
        UpdateClip();
    }

    public void Initialize()
    {
        if (_magnifierHost == null || _target == null)
            return;

        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        _visualSurface = _compositor.CreateVisualSurface();
        _visualSurface.SourceVisual = ElementCompositionPreview.GetElementVisual(_target);
        _visualSurface.SourceSize = new Vector2(SourceSize, SourceSize);

        if (_magnifierVisual == null)
        {
            _magnifierVisual = _compositor.CreateSpriteVisual();
        }

        UpdateVisualSize();

        var brush = _compositor.CreateSurfaceBrush(_visualSurface);
        brush.Stretch = CompositionStretch.UniformToFill;
        _magnifierVisual.Brush = brush;

        UpdateClip();

        ElementCompositionPreview.SetElementChildVisual(_magnifierHost, _magnifierVisual);
    }

    private void UpdateVisualSize()
    {
        if (_magnifierVisual == null)
            return;

        _magnifierVisual.Size = new Vector2((float)ActualWidth, (float)ActualHeight);
    }

    private void UpdateClip()
    {
        if (_magnifierVisual == null || _compositor == null)
            return;

        var ellipse = _compositor.CreateEllipseGeometry();
        ellipse.Center = new Vector2((float)ActualWidth / 2f, (float)ActualHeight / 2f);
        ellipse.Radius = new Vector2((float)ActualWidth / 2f, (float)ActualHeight / 2f);

        var clip = _compositor.CreateGeometricClip(ellipse);
        _magnifierVisual.Clip = clip;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_target == null)
            return;

        _lastPointerPosition = e.GetCurrentPoint(_target).Position;
        _visualSurface.SourceOffset = new Vector2((float)(_lastPointerPosition.X - SourceSize / 2), (float)(_lastPointerPosition.Y - SourceSize / 2));

        Position = GetAdjustedPosition(_lastPointerPosition, new Size(_target.ActualWidth, _target.ActualHeight), new Size(ActualWidth, ActualHeight), Position);

        double offsetX = 0;
        double offsetY = 0;

        switch (Position)
        {
            case MagnifierPosition.Top:
                offsetX = -ActualWidth - SourceSize - SourceSize / 2;
                offsetY = -ActualHeight - ActualHeight - SourceSize;
                break;

            case MagnifierPosition.Bottom:
                offsetX = -ActualWidth - SourceSize - SourceSize / 2;
                offsetY = -ActualHeight + SourceSize;
                break;

            case MagnifierPosition.Left:
                offsetX = -ActualWidth - ActualWidth;
                offsetY = -ActualHeight;
                break;

            case MagnifierPosition.Right:
            default:
                offsetX = -ActualWidth;
                offsetY = -ActualHeight;
                break;
        }

        _translateTransform.X = _lastPointerPosition.X + offsetX + HorizontalOffset;
        _translateTransform.Y = _lastPointerPosition.Y + offsetY + VerticalOffset;
        this.Visibility = Visibility.Visible;
    }
    private MagnifierPosition GetAdjustedPosition(Point pointerPosition, Size targetSize, Size shapeSize, MagnifierPosition preferredPosition)
    {
        double shapeWidth = shapeSize.Width;
        double shapeHeight = shapeSize.Height;

        double targetWidth = targetSize.Width;
        double targetHeight = targetSize.Height;

        double margin = 4;

        switch (preferredPosition)
        {
            case MagnifierPosition.Top:
                if (pointerPosition.Y - shapeHeight < margin)
                    return MagnifierPosition.Bottom;
                break;

            case MagnifierPosition.Bottom:
                if (pointerPosition.Y + shapeHeight > targetHeight - margin)
                    return MagnifierPosition.Top;
                break;

            case MagnifierPosition.Left:
                if (pointerPosition.X - shapeWidth < margin)
                    return MagnifierPosition.Right;
                break;

            case MagnifierPosition.Right:
                if (pointerPosition.X + shapeWidth > targetWidth - margin)
                    return MagnifierPosition.Left;
                break;
        }

        return preferredPosition;
    }
    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        this.Visibility = Visibility.Collapsed;
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_target is FrameworkElement fe)
        {
            fe.PointerMoved -= OnPointerMoved;
            fe.PointerExited -= OnPointerExited;
        }
    }

    public void AttachToTarget(FrameworkElement target)
    {
        _target = target;
        if (_target is FrameworkElement fe)
        {
            fe.PointerMoved += OnPointerMoved;
            fe.PointerExited += OnPointerExited;
        }
        Initialize();
    }
}
