namespace DevWinUI;

[TemplatePart(Name = nameof(PART_HostGrid), Type = typeof(Grid))]
[TemplatePart(Name = nameof(PART_HostGrid), Type = typeof(CanvasAnimatedControl))]
public partial class LiveGraph : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private const string PART_HostGrid = "PART_HostGrid";

    private CanvasAnimatedControl canvas;
    private Grid hostGrid;

    public event EventHandler<float> HighlightLineUpdated;
    public event EventHandler<LiveGraphEventArgs> Draw;
    public event EventHandler<CanvasAnimatedControl> CreateResources;

    private Color drawColor = Colors.Transparent;
    private Color clearColor = Colors.Transparent;

    // current horizontal offset
    private float offsetX = 0;

    // pixels per frame
    private float horizontalScrollSpeed = CalculateSpeed(1, TimeSpan.FromSeconds(1));

    // distance between vertical lines
    private float crossSpacing = 30f;

    // distance between dots
    private float dotSpacing = 6f;

    // Holds the Y position where the line currently is
    private float? currentLineY = null;

    private List<UserPolygon> polygons = new();
    private Dictionary<string, UserPolygon> livePolygons = new();
    private Dictionary<string, GraphBrushData> polygonsBrush = new();

    private TimeSpan highlightLineAnimationDuration = TimeSpan.FromMilliseconds(300);
    private HighlightLineBehavior highlightLineBehavior = HighlightLineBehavior.EachPoint;
    private LiveGraphBackgroundMode backgroundMode = LiveGraphBackgroundMode.Cross;
    private Visibility highlightLineVisibility = Visibility.Visible;
    private object highlightLineContent;

    private int currentPolygonIndex = 0;
    private int currentPointIndex = 0;

    private Compositor compositor;
    private Visual gridVisual;
    public LiveGraph()
    {
        this.DefaultStyleKey = typeof(LiveGraph);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;
        hostGrid = GetTemplateChild(PART_HostGrid) as Grid;
        gridVisual = ElementCompositionPreview.GetElementVisual(hostGrid);
        compositor = gridVisual.Compositor;

        UpdateDrawColor();

        UpdateClearColor();

        UpdateHorizontalScrollSpeed();

        ActualThemeChanged -= OnActualThemeChanged;
        ActualThemeChanged += OnActualThemeChanged;

        canvas.Draw -= OnDraw;
        canvas.Draw += OnDraw;

        canvas.CreateResources -= OnCreateResources;
        canvas.CreateResources += OnCreateResources;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;
    }

    public string RegisterGraphBrush(GraphBrushData brushData)
    {
        var key = Guid.NewGuid().ToString();

        polygonsBrush[key] = brushData;

        return key;
    }
    public void UpdateGraphBrush(string key, GraphBrushData brushData)
    {
        polygonsBrush[key] = brushData;
    }

    public void ResetDynamicGraph(string key)
    {
        if (livePolygons.ContainsKey(key))
        {
            livePolygons.Remove(key);

            var brush = polygonsBrush[key];
            brush?.Brush?.Dispose();
            brush?.OpacityBrush?.Dispose();
            brush?.BorderBrush?.Dispose();

            polygonsBrush.Remove(key);
        }
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        foreach (var item in polygonsBrush)
        {
            item.Value?.Brush?.Dispose();
            item.Value?.OpacityBrush?.Dispose();
            item.Value?.BorderBrush?.Dispose();
            item.Value?.IsDisposed = true;
        }
    }

    private void OnCreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
    {
        CreateResources?.Invoke(this, sender);
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        UpdateDrawColor();
    }
    private void UpdateDrawColor()
    {
        if (BackgroundColor == null)
        {
            drawColor = ActualTheme == ElementTheme.Light
                            ? Microsoft.UI.ColorHelper.FromArgb(32, 0, 0, 0)
                            : Microsoft.UI.ColorHelper.FromArgb(32, 255, 255, 255);
        }
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ResizeGraphPoints((float)e.NewSize.Height, (float)e.PreviousSize.Height);
    }
    private void ResizeGraphPoints(float newHeight, float oldHeight)
    {
        if (oldHeight <= 0) return; // avoid division by zero

        float scaleY = newHeight / oldHeight;

        foreach (var polygon in polygons)
        {
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                var p = polygon.Points[i];
                polygon.Points[i] = new Vector2(p.X, p.Y * scaleY);
            }
        }
    }
    private void UpdateHighlightLine()
    {
        if ((polygons.Count == 0 && livePolygons.Count == 0) || highlightLineVisibility == Visibility.Collapsed || highlightLineContent == null)
            return;

        float canvasWidth = (float)canvas.Size.Width;

        // Combine polygons with livePolygon if exists
        List<UserPolygon> allPolygons = new(polygons);
        if (livePolygons.Count > 0)
            allPolygons.AddRange(livePolygons.Values);

        switch (highlightLineBehavior)
        {
            case HighlightLineBehavior.HigherPointAcrossGraphs:
            case HighlightLineBehavior.LowerPointAcrossGraphs:
                float? targetLineY = currentLineY;

                foreach (var poly in allPolygons)
                {
                    foreach (var pt in poly.Points)
                    {
                        float globalScreenX = pt.X + poly.OffsetX;

                        if (globalScreenX >= 0 && globalScreenX <= canvasWidth)
                        {
                            if (targetLineY == null)
                            {
                                targetLineY = pt.Y;
                            }
                            else
                            {
                                if (highlightLineBehavior == HighlightLineBehavior.HigherPointAcrossGraphs)
                                {
                                    if (pt.Y < targetLineY)
                                        targetLineY = pt.Y;
                                }
                                else
                                {
                                    if (pt.Y > targetLineY)
                                        targetLineY = pt.Y;
                                }
                            }
                        }
                    }
                }

                if (targetLineY.HasValue && targetLineY != currentLineY)
                {
                    currentLineY = targetLineY.Value;
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        HighlightLineUpdated?.Invoke(this, currentLineY.Value);
                        MoveHostGridWithAnimation(currentLineY.Value);
                    });
                }
                break;

            case HighlightLineBehavior.HigherPointPerGraph:
            case HighlightLineBehavior.LowerPointPerGraph:
                if (currentPolygonIndex >= allPolygons.Count) return;

                var polygon = allPolygons[currentPolygonIndex];

                if (currentPointIndex >= polygon.Points.Count)
                {
                    currentPolygonIndex++;
                    currentPointIndex = 0;
                    currentLineY = null;
                    if (currentPolygonIndex >= allPolygons.Count) return;
                    polygon = allPolygons[currentPolygonIndex];
                }

                var point = polygon.Points[currentPointIndex];
                float screenX = point.X + polygon.OffsetX;

                if (screenX >= 0 && screenX <= canvasWidth)
                {
                    float targetY = point.Y;
                    bool shouldMove = currentLineY == null
                        || (highlightLineBehavior == HighlightLineBehavior.HigherPointPerGraph && targetY < currentLineY)
                        || (highlightLineBehavior == HighlightLineBehavior.LowerPointPerGraph && targetY > currentLineY);

                    if (shouldMove)
                    {
                        currentLineY = targetY;

                        _ = DispatcherQueue.TryEnqueue(() =>
                        {
                            HighlightLineUpdated?.Invoke(this, targetY);
                            MoveHostGridWithAnimation(targetY);
                        });
                    }

                    currentPointIndex++;
                }
                break;

            case HighlightLineBehavior.EachPoint:
                if (currentPolygonIndex >= allPolygons.Count) return;

                polygon = allPolygons[currentPolygonIndex];

                if (currentPointIndex >= polygon.Points.Count)
                {
                    currentPolygonIndex++;
                    currentPointIndex = 0;
                    if (currentPolygonIndex >= allPolygons.Count) return;
                    polygon = allPolygons[currentPolygonIndex];
                }

                var nextPoint = polygon.Points[currentPointIndex];
                float nextScreenX = nextPoint.X + polygon.OffsetX;

                if (nextScreenX >= 0 && nextScreenX <= canvasWidth)
                {
                    float nextTargetY = nextPoint.Y;

                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        HighlightLineUpdated?.Invoke(this, nextTargetY);
                        MoveHostGridWithAnimation(nextTargetY);
                    });

                    currentPointIndex++;
                }
                break;
        }
    }
    private void MoveHostGridWithAnimation(double y)
    {
        var animation = compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(1.0f, (float)y, compositor.CreateCubicBezierEasingFunction(new System.Numerics.Vector2(0.0f, 0.0f), new System.Numerics.Vector2(0.2f, 1.0f)));
        animation.Duration = highlightLineAnimationDuration;
        gridVisual.StartAnimation("Offset.Y", animation);
    }

    private void UpdateClearColor()
    {
        canvas.ClearColor = clearColor;
    }
    private float NormalizeY(float percent, float height)
    {
        return height - (percent / 100f) * height;
    }

    private void DrawUserPolygons(CanvasDrawingSession ds, float width, float height)
    {
        foreach (var polygon in polygons)
        {
            if (polygon.Points.Count < 2) continue;

            using (var pathBuilder = new CanvasPathBuilder(ds.Device))
            {
                // Start at the bottom at the first point's X
                float startX = polygon.Points[0].X + polygon.OffsetX;
                pathBuilder.BeginFigure(new Vector2(startX, height)); // bottom

                // Draw up to the first point
                pathBuilder.AddLine(new Vector2(startX, polygon.Points[0].Y));

                if (polygon.IsRounded)
                {
                    Vector2 prev = polygon.Points[0];

                    for (int i = 1; i < polygon.Points.Count; i++)
                    {
                        Vector2 current = polygon.Points[i];
                        Vector2 next = i < polygon.Points.Count - 1 ? polygon.Points[i + 1] : current;

                        // Control points for a smooth curve
                        float smoothness = 0.25f;

                        Vector2 cp1 = new Vector2(
                            prev.X + (current.X - polygon.Points[Math.Max(i - 2, 0)].X) * smoothness,
                            prev.Y + (current.Y - polygon.Points[Math.Max(i - 2, 0)].Y) * smoothness
                        );

                        Vector2 cp2 = new Vector2(
                            current.X - (next.X - prev.X) * smoothness,
                            current.Y - (next.Y - prev.Y) * smoothness
                        );

                        pathBuilder.AddCubicBezier(
                            new Vector2(cp1.X + polygon.OffsetX, cp1.Y),
                            new Vector2(cp2.X + polygon.OffsetX, cp2.Y),
                            new Vector2(current.X + polygon.OffsetX, current.Y)
                        );

                        prev = current;
                    }
                }
                else
                {
                    // Draw polygon lines
                    for (int i = 1; i < polygon.Points.Count; i++)
                    {
                        pathBuilder.AddLine(new Vector2(polygon.Points[i].X + polygon.OffsetX, polygon.Points[i].Y));
                    }
                }

                // Drop straight down to bottom at last point's X
                float endX = polygon.Points.Last().X + polygon.OffsetX;
                pathBuilder.AddLine(new Vector2(endX, height));

                // Do NOT close the figure, keep it open
                pathBuilder.EndFigure(CanvasFigureLoop.Open);

                // Create the geometry
                var geometry = CanvasGeometry.CreatePath(pathBuilder);

                polygonsBrush.TryGetValue(polygon.Key, out var brush);

                if (brush != null)
                {
                    if (brush.IsDisposed)
                        return;

                    try
                    {
                        if (brush.Brush != null && brush.OpacityBrush != null)
                        {
                            ds.FillGeometry(geometry, brush.Brush, brush.OpacityBrush);
                        }
                        else if (brush.Brush != null && brush.OpacityBrush == null)
                        {
                            ds.FillGeometry(geometry, brush.Brush);
                        }

                        if (brush.BorderBrush != null)
                        {
                            ds.DrawGeometry(geometry, brush.BorderBrush, brush.StrokeWidth, brush.StrokeStyle);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                }
            }
        }
    }
    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var ds = args.DrawingSession;
        var width = (float)sender.Size.Width;
        var height = (float)sender.Size.Height;

        // Update offset
        offsetX -= horizontalScrollSpeed;
        if (offsetX <= -crossSpacing)
        {
            offsetX += crossSpacing; // loop offset
        }

        Draw?.Invoke(this, new LiveGraphEventArgs(sender, args));

        DrawBackground(ds, width, height);

        UpdatePolygonsOffset();

        DrawUserPolygons(ds, width, height);

        UpdateHighlightLine(); // <--- this will move the line smoothly
    }
    private void DrawBackground(CanvasDrawingSession ds, float width, float height)
    {
        switch (backgroundMode)
        {
            case LiveGraphBackgroundMode.None:
                break;
            case LiveGraphBackgroundMode.Cross:
                // Draw vertical lines
                for (float x = offsetX; x < width; x += crossSpacing)
                {
                    ds.DrawLine(x, 0, x, height, drawColor, 1f);
                }

                for (float y = 0; y < height; y += crossSpacing)
                {
                    for (float x = offsetX; x < width; x += crossSpacing)
                    {
                        ds.DrawLine(x, y, x + crossSpacing, y, drawColor, 1f);
                    }
                }
                break;
            case LiveGraphBackgroundMode.Dot:
                for (float y = 0; y < height; y += dotSpacing)
                {
                    for (float x = offsetX; x < width; x += dotSpacing)
                    {
                        ds.FillCircle(x, y, 1, drawColor);
                    }
                }
                break;
        }
    }
    private void UpdatePolygonsOffset()
    {
        for (int i = polygons.Count - 1; i >= 0; i--)
        {
            var polygon = polygons[i];
            polygon.OffsetX -= horizontalScrollSpeed;

            // Remove when fully off-screen
            if (polygon.Points.Count > 0 && polygon.Points.Last().X + polygon.OffsetX < 0)
            {
                polygons.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Converts a desired movement over a timespan to a speed in pixels per second.
    /// </summary>
    /// <param name="distance">Distance to move in pixels.</param>
    /// <param name="time">Time to move that distance.</param>
    /// <returns>Speed in pixels per second.</returns>
    private static float CalculateSpeed(float distance, TimeSpan time)
    {
        if (time.TotalSeconds <= 0)
            return 0f; // avoid division by zero

        return distance / (float)time.TotalSeconds;
    }

    public CanvasAnimatedControl GetCanvasAnimatedControl()
    {
        return canvas;
    }
}
