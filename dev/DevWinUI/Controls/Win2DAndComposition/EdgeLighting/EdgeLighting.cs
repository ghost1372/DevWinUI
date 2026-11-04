namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Border), Type = typeof(Border))]
public partial class EdgeLighting : ContentControl
{
    private const string PART_Border = "PART_Border";

    private Border border;

    private Compositor _compositor;
    private CompositionRadialGradientBrush brush;
    private ExpressionAnimation expression;
    private CompositionPropertySet props;
    private ScalarKeyFrameAnimation? progressAnimation;
    private CompositionSpriteShape bgShape;
    private CompositionSpriteShape glowShape;
    private ShapeVisual shapeVisual;
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (border != null)
        {
            border.SizeChanged -= OnSizeChanged;
            DisposeComposition();
        }

        border = GetTemplateChild(PART_Border) as Border;

        if (border != null)
        {
            border.SizeChanged += OnSizeChanged;
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Build();
        UpdateBorderCornerRadius();
    }

    private void UpdateBorderBackground()
    {
        if (bgShape?.StrokeBrush is CompositionColorBrush strokeBrush)
        {
            strokeBrush.Color = BorderStrokeColor;
        }
    }
    private void UpdateStrokeThickness()
    {
        if (bgShape == null || glowShape == null || _compositor == null)
            return;

        bgShape.StrokeThickness = (float)StrokeThickness;
        glowShape.StrokeThickness = (float)StrokeThickness;

    }
    private void DisposeComposition()
    {
        if (border != null)
        {
            Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.SetElementChildVisual(border, null);
        }

        shapeVisual?.Dispose();
        glowShape?.Dispose();
        bgShape?.Dispose();
        brush?.Dispose();
        expression?.Dispose();
        props?.Dispose();
        progressAnimation?.Dispose();

        shapeVisual = null;
        glowShape = null;
        bgShape = null;
        brush = null;
        expression = null;
        props = null;
        progressAnimation = null;
    }
    private void Build()
    {
        DisposeComposition();

        if (border == null)
            return;

        float width = (float)border.ActualWidth;
        float height = (float)border.ActualHeight;

        if (width <= 0f || height <= 0f)
            return;

        _compositor = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(border).Compositor;

        float maxRadius = Math.Min(width, height) / 2f;
        float cornerRadius = IsRounded ? maxRadius : Math.Clamp((float)CornerRadius, 0f, maxRadius);

        // Geometry        
        CompositionRoundedRectangleGeometry roundedRectangleGeometry = null;
        CompositionEllipseGeometry ellipseGeometry = null;
        if (IsRounded)
        {
            ellipseGeometry = _compositor.CreateEllipseGeometry();
            ellipseGeometry.Center = new Vector2(width / 2f, height / 2f);
            ellipseGeometry.Radius = new Vector2(maxRadius, maxRadius);

            bgShape = _compositor.CreateSpriteShape(ellipseGeometry);
            glowShape = _compositor.CreateSpriteShape(ellipseGeometry);
        }
        else
        {
            roundedRectangleGeometry = _compositor.CreateRoundedRectangleGeometry();
            roundedRectangleGeometry.Size = new System.Numerics.Vector2(width, height);
            roundedRectangleGeometry.CornerRadius = new System.Numerics.Vector2(cornerRadius);

            bgShape = _compositor.CreateSpriteShape(roundedRectangleGeometry);
            glowShape = _compositor.CreateSpriteShape(roundedRectangleGeometry);
        }

        // Background border
        bgShape.StrokeBrush = _compositor.CreateColorBrush(BorderStrokeColor);
        bgShape.StrokeThickness = (float)StrokeThickness;
        bgShape.FillBrush = null;

        // Glow brush
        brush = _compositor.CreateRadialGradientBrush();
        brush.MappingMode = CompositionMappingMode.Absolute;

        brush.EllipseRadius = new System.Numerics.Vector2(width, height);

        UpdateBrush();

        // Glow shape
        glowShape.StrokeBrush = brush;
        glowShape.StrokeThickness = (float)StrokeThickness;
        glowShape.FillBrush = null;

        // Shape visual
        shapeVisual = _compositor.CreateShapeVisual();
        shapeVisual.Size = new System.Numerics.Vector2(width, height);

        shapeVisual.Shapes.Add(bgShape);
        shapeVisual.Shapes.Add(glowShape);

        // Clip
        var clipGeo = _compositor.CreateRoundedRectangleGeometry();
        clipGeo.Size = new System.Numerics.Vector2(width, height);
        clipGeo.CornerRadius = new System.Numerics.Vector2(cornerRadius);

        shapeVisual.Clip = _compositor.CreateGeometricClip(clipGeo);

        Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.SetElementChildVisual(border, shapeVisual);

        props = _compositor.CreatePropertySet();
        props.InsertScalar("progress", 0f);

        progressAnimation = _compositor.CreateScalarKeyFrameAnimation();

        // Linear easing
        var linear = _compositor.CreateLinearEasingFunction();
        progressAnimation.InsertKeyFrame(1f, (float)(Math.PI * 2), linear);

        expression = _compositor.CreateExpressionAnimation(
            "Vector2(cx + cos(props.progress) * r, cy + sin(props.progress) * r)");

        expression.SetReferenceParameter("props", props);
        expression.SetScalarParameter("cx", width / 2);
        expression.SetScalarParameter("cy", height / 2);
        expression.SetScalarParameter("r", maxRadius);

        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        if (progressAnimation == null)
            return;

        progressAnimation.Duration = Duration > TimeSpan.Zero ? Duration : TimeSpan.FromMilliseconds(1);
        progressAnimation.IterationBehavior = IterationBehavior;
        progressAnimation.IterationCount = Math.Max(1, IterationCount);

        UpdateLightingState();
    }
    private void UpdateBrush()
    {
        if (brush == null || _compositor == null)
            return;

        var stops = brush.ColorStops;
        if (stops.Count != 4)
        {
            stops.Clear();
            for (int i = 0; i < 4; i++)
            {
                stops.Add(_compositor.CreateColorGradientStop());
            }
        }

        SetStop(stops[0], HighlightOffset, HighlightColor);
        SetStop(stops[1], MidOffset, MidColor);
        SetStop(stops[2], FadeOffset, FadeColor);
        SetStop(stops[3], 1d, TailColor);
    }
    private static void SetStop(CompositionColorGradientStop stop, double offset, Color color)
    {
        stop.Offset = Math.Clamp((float)offset, 0f, 1f);
        stop.Color = color;
    }
    private void UpdateLightingState()
    {
        if (brush == null || expression == null || props == null || progressAnimation == null)
            return;

        if (IsLighting)
        {
            props.StartAnimation("progress", progressAnimation);

            brush.StartAnimation(nameof(brush.EllipseCenter), expression);
        }
        else
        {
            props.StopAnimation("progress");

            brush.StopAnimation(nameof(brush.EllipseCenter));
        }
    }

    private void UpdateBorderCornerRadius()
    {
        if (border == null)
            return;

        if (IsRounded)
        {
            double gridRadius = Math.Min(border.ActualWidth, border.ActualHeight) / 2;
            border.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(gridRadius);
        }
        else
        {
            border.CornerRadius = new CornerRadius(Math.Max(0d, CornerRadius));
        }
    }
}
