using Microsoft.UI.Xaml.Automation;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = RootPartName, Type = typeof(Border))]
[TemplatePart(Name = PatternPartName, Type = typeof(Path))]
[TemplatePart(Name = CanvasPartName, Type = typeof(CanvasControl))]
[TemplateVisualState(Name = WideStateName, GroupName = LayoutGroupName)]
[TemplateVisualState(Name = MediumStateName, GroupName = LayoutGroupName)]
[TemplateVisualState(Name = NarrowStateName, GroupName = LayoutGroupName)]
[TemplateVisualState(Name = InfoVisibleStateName, GroupName = InfoGroupName)]
[TemplateVisualState(Name = InfoHiddenStateName, GroupName = InfoGroupName)]
public sealed partial class FlightStatusCard : Control
{
    private const string RootPartName = "PART_Root";
    private const string PatternPartName = "PART_Pattern";
    private const string CanvasPartName = "PART_Canvas";
    private const string LayoutGroupName = "LayoutStates";
    private const string WideStateName = "Wide";
    private const string MediumStateName = "Medium";
    private const string NarrowStateName = "Narrow";
    private const string InfoGroupName = "InfoStates";
    private const string InfoVisibleStateName = "InfoVisible";
    private const string InfoHiddenStateName = "InfoHidden";

    private const double WideMinWidth = 640;
    private const double MediumMinWidth = 440;
    private const double InfoMinHeight = 410;
    private const double AdaptiveMinWidth = 320;
    private const double MinScale = 0.1;
    private const double MaxDpiScale = 4;

    private const double PatternPitch = 12;
    private const double PatternDotSize = 2;
    private const double PatternMargin = 6;
    private const double AnimationDuration = 1200;
    private const float GlowBlurAmount = 15;
    private const float GlowOpacity = 0.6f;
    private const float BadgeSize = 38;
    private const float BadgeMargin = 10;
    private const float GlyphScale = 0.85f;

    private static readonly CanvasStrokeStyle s_glyphStroke = new() { LineJoin = CanvasLineJoin.Round };

    private static readonly Vector2[] s_planePoints =
    {
        new(11, 0), new(8.5f, -1.2f), new(2.5f, -1.4f), new(-2.2f, -9), new(-4.6f, -9),
        new(-3.8f, -1.5f), new(-6.8f, -1.3f), new(-9, -4.2f), new(-10.8f, -4.2f), new(-10.4f, -1),
        new(-10.4f, 1), new(-10.8f, 4.2f), new(-9, 4.2f), new(-6.8f, 1.3f), new(-3.8f, 1.5f),
        new(-4.6f, 9), new(-2.2f, 9), new(2.5f, 1.4f), new(8.5f, 1.2f),
    };

    private Border? _root;
    private Path? _pattern;
    private CanvasControl? _canvas;
    private CanvasLinearGradientBrush? _gradient;
    private RenderModel? _gradientModel;
    private CanvasGeometry? _clip;
    private CanvasGeometry? _plane;
    private Vector2 _clipSize;
    private ShadowEffect? _shadow;
    private RenderModel? _model;
    private double _displayed;
    private double _from;
    private double _to;
    private TimeSpan _start;
    private bool _hasStart;
    private Size _natural;
    private string? _layoutState;
    private string? _infoState;
    private ScaleTransform? _scaleTransform;
    private bool _animating;
    private bool _loaded;

    public FlightStatusCard()
    {
        DefaultStyleKey = typeof(FlightStatusCard);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        Refresh();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_root is not null)
        {
            _root.SizeChanged -= OnRootSizeChanged;
        }

        if (_canvas is not null)
        {
            _canvas.Draw -= OnDraw;
            _canvas.CreateResources -= OnCreateResources;
        }

        _layoutState = null;
        _infoState = null;
        _root = GetTemplateChild(RootPartName) as Border;
        _pattern = GetTemplateChild(PatternPartName) as Path;
        _canvas = GetTemplateChild(CanvasPartName) as CanvasControl;

        if (_pattern is not null)
        {
            _pattern.StrokeThickness = PatternDotSize;
            _pattern.StrokeDashArray = new DoubleCollection { 0, PatternPitch / PatternDotSize };
        }

        if (_root is not null)
        {
            _root.SizeChanged += OnRootSizeChanged;
        }

        if (_canvas is not null)
        {
            _canvas.Draw += OnDraw;
            _canvas.CreateResources += OnCreateResources;
        }

        LayoutPattern();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (ResizeMode == FlightStatusCardResizeMode.Scale)
        {
            _natural = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
            {
                return _natural;
            }
        }

        double scale = GetScale(availableSize);
        Size desired = base.MeasureOverride(new Size(availableSize.Width / scale, availableSize.Height / scale));
        return new Size(desired.Width * scale, desired.Height * scale);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double scale = GetScale(finalSize);
        ApplyScale(scale);
        base.ArrangeOverride(new Size(finalSize.Width / scale, finalSize.Height / scale));
        return finalSize;
    }

    private double GetScale(Size size)
    {
        if (ResizeMode == FlightStatusCardResizeMode.Adaptive)
        {
            return double.IsInfinity(size.Width) ? 1 : Math.Clamp(size.Width / AdaptiveMinWidth, MinScale, 1);
        }

        if (_natural.Width <= 0 || _natural.Height <= 0)
        {
            return 1;
        }

        double scale = double.PositiveInfinity;

        if (!double.IsInfinity(size.Width))
        {
            scale = size.Width / _natural.Width;
        }

        if (!double.IsInfinity(size.Height))
        {
            scale = Math.Min(scale, size.Height / _natural.Height);
        }

        return double.IsInfinity(scale) ? 1 : Math.Max(scale, MinScale);
    }

    private void ApplyScale(double scale)
    {
        if (_canvas is not null)
        {
            float dpiScale = (float)Math.Clamp(Math.Ceiling(scale * 4) / 4, 1, MaxDpiScale);

            if (_canvas.DpiScale != dpiScale)
            {
                _canvas.DpiScale = dpiScale;
            }
        }

        if (_root is null)
        {
            return;
        }

        if (Math.Abs(scale - 1) < 0.001)
        {
            if (_root.RenderTransform is not null)
            {
                _root.RenderTransform = null;
            }

            return;
        }

        _scaleTransform ??= new ScaleTransform();
        _scaleTransform.ScaleX = scale;
        _scaleTransform.ScaleY = scale;

        if (!ReferenceEquals(_root.RenderTransform, _scaleTransform))
        {
            _root.RenderTransform = _scaleTransform;
        }
    }

    private static double Clamp(double value) => Math.Clamp(double.IsNaN(value) ? 0 : value, 0, 100);

    private static double GetCornerClearance(double y, double height, double radius)
    {
        double dy = y < radius ? radius - y : y > height - radius ? y - (height - radius) : 0;
        if (dy <= 0)
        {
            return 0;
        }

        return dy >= radius ? radius : radius - Math.Sqrt(radius * radius - dy * dy);
    }

    private static Color GetColor(Brush? brush)
    {
        if (brush is not SolidColorBrush solid)
        {
            return default;
        }

        Color color = solid.Color;
        color.A = (byte)(color.A * solid.Opacity);
        return color;
    }

    private static CanvasGradientStop[]? GetStops(Brush? brush, out Vector2 start, out Vector2 end)
    {
        start = default;
        end = default;

        if (brush is not LinearGradientBrush gradient || gradient.GradientStops.Count == 0)
        {
            return null;
        }

        var stops = new CanvasGradientStop[gradient.GradientStops.Count];
        for (int i = 0; i < stops.Length; i++)
        {
            GradientStop stop = gradient.GradientStops[i];
            Color color = stop.Color;
            color.A = (byte)(color.A * gradient.Opacity);
            stops[i] = new CanvasGradientStop { Position = (float)stop.Offset, Color = color };
        }

        Array.Sort(stops, static (a, b) => a.Position.CompareTo(b.Position));

        start = new Vector2((float)gradient.StartPoint.X, (float)gradient.StartPoint.Y);
        end = new Vector2((float)gradient.EndPoint.X, (float)gradient.EndPoint.Y);
        return stops;
    }

    private static CanvasGeometry CreatePlane(ICanvasResourceCreator resourceCreator)
    {
        Matrix3x2 transform = Matrix3x2.CreateRotation(-MathF.PI / 4) * Matrix3x2.CreateScale(GlyphScale);

        using var builder = new CanvasPathBuilder(resourceCreator);
        builder.BeginFigure(Vector2.Transform(s_planePoints[0], transform));

        for (int i = 1; i < s_planePoints.Length; i++)
        {
            builder.AddLine(Vector2.Transform(s_planePoints[i], transform));
        }

        builder.EndFigure(CanvasFigureLoop.Closed);
        return CanvasGeometry.CreatePath(builder);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        AnimateTo(Clamp(Progress), fromZero: true);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        StopAnimation();
        ReleaseResources();
    }

    private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateLayoutState(e.NewSize);
        LayoutPattern();
    }

    private void UpdateLayoutState(Size size)
    {
        if (size.Width <= 0 || size.Height <= 0)
        {
            return;
        }

        bool adaptive = ResizeMode == FlightStatusCardResizeMode.Adaptive;
        bool compact = adaptive && size.Width < WideMinWidth;

        string layout = !compact
            ? WideStateName
            : size.Width >= MediumMinWidth ? MediumStateName : NarrowStateName;

        string info = compact && size.Height < InfoMinHeight
            ? InfoHiddenStateName
            : InfoVisibleStateName;

        GoToState(ref _layoutState, layout);
        GoToState(ref _infoState, info);
    }

    private void GoToState(ref string? current, string state)
    {
        if (state == current)
        {
            return;
        }

        current = state;
        VisualStateManager.GoToState(this, state, false);
    }

    private void OnCreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        => ReleaseResources();

    private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var card = (FlightStatusCard)d;
        card._layoutState = null;
        card._infoState = null;
        card.UpdateLayoutState(new Size(card._root?.ActualWidth ?? 0, card._root?.ActualHeight ?? 0));
        card.InvalidateMeasure();
    }

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((FlightStatusCard)d).Refresh();

    private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((FlightStatusCard)d).UpdateModel();

    private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var card = (FlightStatusCard)d;
        card.Refresh();

        if (card._loaded)
        {
            card.AnimateTo(Clamp((double)e.NewValue), fromZero: false);
        }
    }

    private void AnimateTo(double target, bool fromZero)
    {
        double from = fromZero ? 0 : _displayed;

        StopAnimation();

        if (Math.Abs(from - target) < 0.01)
        {
            _displayed = target;
            Redraw();
            return;
        }

        _displayed = from;
        _from = from;
        _to = target;
        _hasStart = false;
        _animating = true;
        CompositionTarget.Rendering += OnRendering;
        Redraw();
    }

    private void StopAnimation()
    {
        if (!_animating)
        {
            return;
        }

        _animating = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        TimeSpan now = ((RenderingEventArgs)e).RenderingTime;

        if (!_hasStart)
        {
            _start = now;
            _hasStart = true;
        }

        double t = Math.Min(1, (now - _start).TotalMilliseconds / AnimationDuration);
        double eased = 1 - Math.Pow(1 - t, 3);
        _displayed = _from + (_to - _from) * eased;
        Redraw();

        if (t >= 1)
        {
            StopAnimation();
        }
    }

    private void Redraw() => _canvas?.Invalidate();

    private void ReleaseResources()
    {
        _gradient?.Dispose();
        _gradient = null;
        _gradientModel = null;
        _clip?.Dispose();
        _clip = null;
        _plane?.Dispose();
        _plane = null;
        _shadow?.Dispose();
        _shadow = null;
    }

    private void Refresh()
    {
        AutomationProperties.SetName(this,
            $"Flight from {DepartureCity} {DepartureCode} to {ArrivalCity} {ArrivalCode}, {Clamp(Progress):0}% complete");
    }

    private void UpdateModel()
    {
        Color glow = GetColor(GlowBrush);
        glow.A = (byte)(glow.A * GlowOpacity);

        CanvasGradientStop[]? stops = GetStops(ProgressBrush, out Vector2 start, out Vector2 end);

        _model = new RenderModel(
            GetColor(ProgressBrush),
            stops,
            start,
            end,
            glow,
            GetColor(BadgeBrush),
            GetColor(BadgeGlyphBrush));

        Redraw();
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        RenderModel? model = _model;
        float trackWidth = (float)sender.ActualWidth;
        float trackHeight = (float)sender.ActualHeight;

        if (model is null || trackWidth <= 0 || trackHeight <= 0)
        {
            return;
        }

        float progress = (float)Math.Clamp(_displayed, 0, 100);
        float width = MathF.Min(trackWidth, MathF.Max(trackHeight, trackWidth * progress / 100f));
        float radius = trackHeight / 2;
        CanvasDrawingSession session = args.DrawingSession;

        if (_clip is null || _clipSize.X != trackWidth || _clipSize.Y != trackHeight)
        {
            _clip?.Dispose();
            _clip = CanvasGeometry.CreateRoundedRectangle(sender, 0, 0, trackWidth, trackHeight, radius, radius);
            _clipSize = new Vector2(trackWidth, trackHeight);
        }

        using (session.CreateLayer(1f, _clip))
        {
            if (model.Glow.A > 0)
            {
                DrawGlow(sender, session, model, width, trackHeight, radius);
            }

            DrawBar(sender, session, model, width, trackHeight, radius);
            DrawBadge(sender, session, model, width, trackHeight);
        }
    }

    private void DrawGlow(CanvasControl sender, CanvasDrawingSession session, RenderModel model, float width, float height, float radius)
    {
        using var commands = new CanvasCommandList(sender);

        using (CanvasDrawingSession shape = commands.CreateDrawingSession())
        {
            shape.FillRoundedRectangle(0, 0, width, height, radius, radius, Colors.White);
        }

        _shadow ??= new ShadowEffect { BlurAmount = GlowBlurAmount };
        _shadow.Source = commands;
        _shadow.ShadowColor = model.Glow;
        session.DrawImage(_shadow);
        _shadow.Source = null;
    }

    private void DrawBar(CanvasControl sender, CanvasDrawingSession session, RenderModel model, float width, float height, float radius)
    {
        if (model.Stops is not { Length: > 0 } stops)
        {
            session.FillRoundedRectangle(0, 0, width, height, radius, radius, model.Fill);
            return;
        }

        if (!ReferenceEquals(_gradientModel, model))
        {
            _gradient?.Dispose();
            _gradient = new CanvasLinearGradientBrush(sender, stops);
            _gradientModel = model;
        }

        _gradient!.StartPoint = new Vector2(model.GradientStart.X * width, model.GradientStart.Y * height);
        _gradient.EndPoint = new Vector2(model.GradientEnd.X * width, model.GradientEnd.Y * height);

        session.FillRoundedRectangle(0, 0, width, height, radius, radius, _gradient);
    }

    private void DrawBadge(CanvasControl sender, CanvasDrawingSession session, RenderModel model, float width, float height)
    {
        const float half = BadgeSize / 2;
        float centerX = width - BadgeMargin - half;
        float centerY = height / 2;

        session.FillCircle(centerX, centerY, half, model.Badge);

        _plane ??= CreatePlane(sender);

        Matrix3x2 previous = session.Transform;
        session.Transform = Matrix3x2.CreateTranslation(centerX, centerY) * previous;
        session.FillGeometry(_plane, model.GlyphColor);
        session.DrawGeometry(_plane, model.GlyphColor, 1f, s_glyphStroke);
        session.Transform = previous;
    }

    private void LayoutPattern()
    {
        if (_root is null || _pattern is null)
        {
            return;
        }

        double width = _root.ActualWidth;
        double height = _root.ActualHeight;

        if (width <= 0 || height <= 0)
        {
            _pattern.Data = null;
            return;
        }

        double radius = _root.CornerRadius.TopLeft;
        double half = PatternPitch / 2;
        var group = new GeometryGroup();

        for (double y = half; y <= height - PatternMargin; y += PatternPitch)
        {
            double clearance = GetCornerClearance(y, height, radius) + PatternMargin;
            double start = half + Math.Ceiling(Math.Max(0, clearance - half) / PatternPitch) * PatternPitch;
            double end = width - clearance;

            if (end <= start)
            {
                continue;
            }

            group.Children.Add(new LineGeometry { StartPoint = new Point(start, y), EndPoint = new Point(end, y) });
        }

        _pattern.Data = group;
    }

    private sealed record RenderModel(
        Color Fill,
        CanvasGradientStop[]? Stops,
        Vector2 GradientStart,
        Vector2 GradientEnd,
        Color Glow,
        Color Badge,
        Color GlyphColor);
}
