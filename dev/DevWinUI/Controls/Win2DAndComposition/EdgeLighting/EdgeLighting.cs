namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Border), Type = typeof(Grid))]
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
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        border = GetTemplateChild(PART_Border) as Border;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Build();
        UpdateBorderCornerRadius();
    }

    private void UpdateBorderBackground()
    {
        if (bgShape == null || _compositor == null)
            return;

        bgShape.StrokeBrush = _compositor.CreateColorBrush(BorderStrokeColor);
    }
    private void UpdateStrokeThickness()
    {
        if (bgShape == null || glowShape == null || _compositor == null)
            return;

        bgShape.StrokeThickness = (float)StrokeThickness;
        glowShape.StrokeThickness = (float)StrokeThickness;

    }
    private void Build()
    {
        if (border == null)
            return;

        _compositor = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(border).Compositor;

        float width = (float)border.ActualWidth;
        float height = (float)border.ActualHeight;

        // Geometry        
        CompositionRoundedRectangleGeometry roundedRectangleGeometry = null;
        CompositionEllipseGeometry ellipseGeometry = null;
        if (IsRounded)
        {
            float radius = Math.Min(width, height) / 2f;
            ellipseGeometry = _compositor.CreateEllipseGeometry();
            ellipseGeometry.Center = new Vector2(width / 2f, height / 2f);
            ellipseGeometry.Radius = new Vector2(radius, radius);

            bgShape = _compositor.CreateSpriteShape(ellipseGeometry);
            glowShape = _compositor.CreateSpriteShape(ellipseGeometry);
        }
        else
        {
            roundedRectangleGeometry = _compositor.CreateRoundedRectangleGeometry();
            roundedRectangleGeometry.Size = new System.Numerics.Vector2(width, height);
            roundedRectangleGeometry.CornerRadius = new System.Numerics.Vector2((float)CornerRadius);

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
        var shapeVisual = _compositor.CreateShapeVisual();
        shapeVisual.Size = new System.Numerics.Vector2(width, height);

        shapeVisual.Shapes.Add(bgShape);
        shapeVisual.Shapes.Add(glowShape);

        // Clip
        var clipGeo = _compositor.CreateRoundedRectangleGeometry();
        clipGeo.Size = new System.Numerics.Vector2(width, height);
        clipGeo.CornerRadius = new System.Numerics.Vector2((float)CornerRadius);

        shapeVisual.Clip = _compositor.CreateGeometricClip(clipGeo);

        Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.SetElementChildVisual(border, shapeVisual);

        props = _compositor.CreatePropertySet();
        props.InsertScalar("progress", 0f);

        progressAnimation = _compositor.CreateScalarKeyFrameAnimation();

        // Linear easing
        var linear = _compositor.CreateLinearEasingFunction();
        progressAnimation.InsertKeyFrame(1f, (float)(Math.PI * 2), linear);
        progressAnimation.Duration = Duration;
        progressAnimation.IterationBehavior = IterationBehavior;
        progressAnimation.IterationCount = IterationCount;

        float radiusParameter = Math.Min(width, height) / 2;

        expression = _compositor.CreateExpressionAnimation(
            "Vector2(cx + cos(props.progress) * r, cy + sin(props.progress) * r)");

        expression.SetReferenceParameter("props", props);
        expression.SetScalarParameter("cx", width / 2);
        expression.SetScalarParameter("cy", height / 2);
        expression.SetScalarParameter("r", radiusParameter);

        UpdateLightingState();
    }
    private void UpdateBrush()
    {
        if (brush == null || _compositor == null)
            return;

        var highlight = _compositor.CreateColorGradientStop((float)HighlightOffset, HighlightColor);
        var mid = _compositor.CreateColorGradientStop((float)MidOffset, MidColor);
        var fade = _compositor.CreateColorGradientStop((float)FadeOffset, FadeColor);
        var transparent = _compositor.CreateColorGradientStop(1f, TailColor);

        brush.ColorStops.Clear();
        brush.ColorStops.Add(highlight);
        brush.ColorStops.Add(mid);
        brush.ColorStops.Add(fade);
        brush.ColorStops.Add(transparent);
    }
    private void UpdateLightingState()
    {
        if (brush == null || expression == null || props == null || progressAnimation == null)
            return;

        if (IsLighting)
        {
            props.StartAnimation("progress", progressAnimation);

            brush.StartAnimation(nameof(brush.CenterPoint), expression);
            brush.StartAnimation(nameof(brush.EllipseCenter), expression);
        }
        else
        {
            props.StopAnimation("progress");

            brush.StopAnimation(nameof(brush.CenterPoint));
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
            border.CornerRadius = new CornerRadius(CornerRadius);
        }
    }
}
