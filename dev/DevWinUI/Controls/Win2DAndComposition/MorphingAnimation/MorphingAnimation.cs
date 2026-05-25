namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasAnimatedControl))]
public partial class MorphingAnimation : Control
{
    private const string PART_Canvas = "PART_Canvas";

    private CanvasAnimatedControl canvas;

    private Windows.UI.Color primaryColor = Windows.UI.Color.FromArgb(255, 0x1D, 0xA1, 0xF2);
    private Windows.UI.Color secondaryColor = Windows.UI.Color.FromArgb(255, 0x18, 0x77, 0xF2);

    private string primaryData;
    private string secondaryData;

    private enum MorphingAnimationPhase { ToSecondary, HoldSecondary, ToPrimary, HoldPrimary }

    private double animDuration = 0.5;  // seconds – morph travel time
    private double holdDuration = 1.0;  // seconds – pause at each end

    private List<Vector2> primaryShape;
    private List<Vector2> secondaryShape;

    private MorphingAnimationPhase phase = MorphingAnimationPhase.ToSecondary;
    private double phaseTime = 0.0;
    private float progress = 0f;    // 0 = Primary, 1 = Secondary

    private int pointCount = 300;

    public MorphingAnimation()
    {
        DefaultStyleKey = typeof(MorphingAnimation);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;

        GetShapesPoint();

        canvas.Draw -= OnDraw;
        canvas.Draw += OnDraw;

        canvas.Update -= OnUpdate;
        canvas.Update += OnUpdate;
    }

    private void GetShapesPoint()
    {
        primaryShape = SamplePointsOnPath(primaryData, pointCount);
        secondaryShape = SamplePointsOnPath(secondaryData, pointCount);
    }

    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        double dt = args.Timing.ElapsedTime.TotalSeconds;

        phaseTime += dt;

        switch (phase)
        {
            case MorphingAnimationPhase.ToSecondary:
                progress = (float)Math.Min(phaseTime / animDuration, 1.0);
                if (phaseTime >= animDuration)
                {
                    progress = 1f;
                    phaseTime = 0.0;
                    phase = MorphingAnimationPhase.HoldSecondary;
                }
                break;

            case MorphingAnimationPhase.HoldSecondary:
                if (phaseTime >= holdDuration)
                {
                    phaseTime = 0.0;
                    phase = MorphingAnimationPhase.ToPrimary;
                }
                break;

            case MorphingAnimationPhase.ToPrimary:
                progress = (float)Math.Max(1.0 - phaseTime / animDuration, 0.0);
                if (phaseTime >= animDuration)
                {
                    progress = 0f;
                    phaseTime = 0.0;
                    phase = MorphingAnimationPhase.HoldPrimary;
                }
                break;

            case MorphingAnimationPhase.HoldPrimary:
                if (phaseTime >= holdDuration)
                {
                    phaseTime = 0.0;
                    phase = MorphingAnimationPhase.ToSecondary;
                }
                break;
        }
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var ds = args.DrawingSession;

        float eased = EaseInOut(progress);

        if (primaryShape == null || secondaryShape == null)
            return;

        var morphed = Morph(primaryShape, secondaryShape, eased);

        // Scale and center the morphed points to always fit the current canvas size.
        var fitted = FitToCanvas(morphed, (float)sender.Size.Width, (float)sender.Size.Height);

        using var builder = new CanvasPathBuilder(sender.Device);

        builder.BeginFigure(fitted[0]);
        for (int i = 1; i < fitted.Count; i++)
            builder.AddLine(fitted[i]);
        builder.EndFigure(CanvasFigureLoop.Closed);

        using var geometry = CanvasGeometry.CreatePath(builder);

        var fillColor = LerpColor(primaryColor, secondaryColor, eased);
        ds.FillGeometry(geometry, fillColor);
    }

    /// <summary>
    /// Uniformly scales and centers <paramref name="points"/> to fit inside the given canvas
    /// dimensions with a small padding. Because it reads <c>sender.Size</c> every frame,
    /// this also handles any SizeChanged automatically.
    /// </summary>
    private static List<Vector2> FitToCanvas(List<Vector2> points, float canvasWidth, float canvasHeight, float padding = 8f)
    {
        if (points == null || points.Count == 0 || canvasWidth <= 0 || canvasHeight <= 0)
            return points;

        float minX = points[0].X, maxX = points[0].X;
        float minY = points[0].Y, maxY = points[0].Y;

        foreach (var p in points)
        {
            if (p.X < minX) minX = p.X;
            if (p.X > maxX) maxX = p.X;
            if (p.Y < minY) minY = p.Y;
            if (p.Y > maxY) maxY = p.Y;
        }

        float geomW = maxX - minX;
        float geomH = maxY - minY;

        float availW = canvasWidth  - padding * 2f;
        float availH = canvasHeight - padding * 2f;

        // Uniform scale so the shape fits without distortion.
        float scale = (geomW > 0 && geomH > 0)
            ? Math.Min(availW / geomW, availH / geomH)
            : 1f;

        // Offset to center the scaled shape on the canvas.
        float offsetX = padding + (availW - geomW * scale) / 2f - minX * scale;
        float offsetY = padding + (availH - geomH * scale) / 2f - minY * scale;

        var result = new List<Vector2>(points.Count);
        foreach (var p in points)
            result.Add(new Vector2(p.X * scale + offsetX, p.Y * scale + offsetY));

        return result;
    }

    private static List<Vector2> SamplePointsOnPath(string svgPath, int pointCount)
    {
        if (string.IsNullOrEmpty(svgPath))
            return null;

        var device = CanvasDevice.GetSharedDevice();
        using var geometry = CanvasObject.CreateGeometry(svgPath);
        float length = geometry.ComputePathLength();

        var points = new List<Vector2>(pointCount);
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            points.Add(geometry.ComputePointOnPath(t * length));
        }
        return points;
    }

    private static Windows.UI.Color LerpColor(Windows.UI.Color a, Windows.UI.Color b, float t) => Windows.UI.Color.FromArgb(
            (byte)(a.A + (b.A - a.A) * t),
            (byte)(a.R + (b.R - a.R) * t),
            (byte)(a.G + (b.G - a.G) * t),
            (byte)(a.B + (b.B - a.B) * t));

    private static float EaseInOut(float t) => t * t * (3f - 2f * t);

    private static List<Vector2> Morph(List<Vector2> a, List<Vector2> b, float t)
    { 
        var result = new List<Vector2>(a.Count);

        for (int i = 0; i < a.Count; i++)
        {
            result.Add(Vector2.Lerp(a[i], b[i], t));
        }

        return result;
    }
}
