namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Border), Type = typeof(Border))]
public partial class AnimationPath : Control
{
    private const string PART_Border = "PART_Border";
    private Border hostBorder;

    private Compositor _compositor;
    private CompositionSpriteShape _shape;
    private ShapeVisual _shapeVisual;
    private float _length;
    private Rect bounds;
    private CanvasGeometry _baseGeometry;
    private CompositionPath _compositionPath;
    private ScalarKeyFrameAnimation _strokeAnimation;
    public AnimationPath()
    {
        DefaultStyleKey = typeof(AnimationPath);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        hostBorder = GetTemplateChild(PART_Border) as Border;

        Init();
    }

    private void Init()
    {
        if (string.IsNullOrEmpty(Data))
            return;

        _compositor = ElementCompositionPreview.GetElementVisual(hostBorder).Compositor;

        _baseGeometry = CanvasObject.CreateGeometry(Data);

        bounds = _baseGeometry.ComputeBounds();
        _length = _baseGeometry.ComputePathLength();

        _compositionPath = new CompositionPath(_baseGeometry);

        var pathGeometry = _compositor.CreatePathGeometry(_compositionPath);
        _shape = _compositor.CreateSpriteShape(pathGeometry);
        Color color = GetColorFromBrush();

        _shape.StrokeBrush = _compositor.CreateColorBrush(color);
        _shape.StrokeThickness = (float)StrokeThickness;

        _shape.StrokeDashCap = StrokeDashCap;
        _shape.StrokeDashOffset = (float)StrokeDashOffset;
        _shape.StrokeEndCap = StrokeEndCap;
        _shape.StrokeLineJoin = StrokeLineJoin;
        _shape.StrokeMiterLimit = (float)StrokeMiterLimit;
        _shape.StrokeStartCap = StrokeStartCap;
        
        _shape.StrokeDashArray.Add(_length);
        _shape.StrokeDashArray.Add(_length);
        _shape.StrokeDashOffset = _length;

        _shapeVisual = _compositor.CreateShapeVisual();
        _shapeVisual.Shapes.Add(_shape);
        ElementCompositionPreview.SetElementChildVisual(hostBorder, _shapeVisual);

        AnimateStroke(_length);

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        UpdateLayoutOnly();
    }

    private Color GetColorFromBrush()
    {
        Color color = Application.Current.Resources["SystemAccentColor"] is Color c ? c : Colors.Red;
        if (Foreground is SolidColorBrush brush)
        {
            color = brush.Color;
        }

        return color;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateLayoutOnly();
    }

    private void UpdateLayoutOnly()
    {
        if (_baseGeometry == null || _shapeVisual == null)
            return;

        float targetWidth = (float)ActualWidth;
        float targetHeight = (float)ActualHeight;

        if (targetWidth <= 0 || targetHeight <= 0)
            return;

        float strokePadding = (float)StrokeThickness;

        float availableWidth = targetWidth - strokePadding;
        float availableHeight = targetHeight - strokePadding;

        float scale = MathF.Min(
            availableWidth / (float)bounds.Width,
            availableHeight / (float)bounds.Height
        );

        float scaledWidth = (float)bounds.Width * scale;
        float scaledHeight = (float)bounds.Height * scale;

        float offsetX = (targetWidth - scaledWidth) / 2f;
        float offsetY = (targetHeight - scaledHeight) / 2f;

        _shape.TransformMatrix =
            Matrix3x2.CreateTranslation(-(float)bounds.X, -(float)bounds.Y) *
            Matrix3x2.CreateScale(scale) *
            Matrix3x2.CreateTranslation(offsetX, offsetY);

        _shapeVisual.Size = new Vector2(
            targetWidth + (float)StrokeThickness,
            targetHeight + (float)StrokeThickness);
    }
    private void AnimateStroke(float length)
    {
        if (_compositor == null)
            return;

        var linearEasing = _compositor.CreateLinearEasingFunction();

        _strokeAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _strokeAnimation.InsertKeyFrame(0f, length, linearEasing);
        _strokeAnimation.InsertKeyFrame(1f, -length, linearEasing);

        _strokeAnimation.Duration = Duration;
        _strokeAnimation.IterationBehavior = RepeatBehavior;

        HandleAnimationState();
    }

    private void HandleAnimationState()
    {
        if (_shape == null)
            return;

        if (IsPlaying)
        {
            _shape.StartAnimation(nameof(_shape.StrokeDashOffset), _strokeAnimation);
        }
        else
        {
            _shape.StopAnimation(nameof(_shape.StrokeDashOffset));
        }
    }
}
