using System.Diagnostics;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = CanvasPartName, Type = typeof(CanvasControl))]
[TemplatePart(Name = ToolTipPartName, Type = typeof(Border))]
[TemplatePart(Name = ToolTipTextPartName, Type = typeof(TextBlock))]
[TemplatePart(Name = ZoomPanelPartName, Type = typeof(FrameworkElement))]
[TemplatePart(Name = ZoomInPartName, Type = typeof(ButtonBase))]
[TemplatePart(Name = ZoomOutPartName, Type = typeof(ButtonBase))]
[TemplatePart(Name = ResetPartName, Type = typeof(ButtonBase))]
public partial class WorldMap : Control
{
    private const string CanvasPartName = "PART_Canvas";
    private const string ToolTipPartName = "PART_ToolTip";
    private const string ToolTipTextPartName = "PART_ToolTipText";
    private const string ZoomPanelPartName = "PART_ZoomPanel";
    private const string ZoomInPartName = "PART_ZoomInButton";
    private const string ZoomOutPartName = "PART_ZoomOutButton";
    private const string ResetPartName = "PART_ResetButton";

    private const float ContentPadding = 12f;
    private const double DragThreshold = 4;

    private static readonly CanvasStrokeStyle SolidStyle = new() { LineJoin = CanvasLineJoin.Round };
    private static readonly CanvasStrokeStyle DashedStyle = new() { LineJoin = CanvasLineJoin.Round, CustomDashStyle = new float[] { 4f, 3f } };
    private static readonly CanvasStrokeStyle DottedStyle = new() { LineJoin = CanvasLineJoin.Round, DashCap = CanvasCapStyle.Round, CustomDashStyle = new float[] { 0.1f, 2.5f } };

    private static CanvasStrokeStyle GetStrokeStyle(WorldMapBorderStyle style) => style switch
    {
        WorldMapBorderStyle.Dashed => DashedStyle,
        WorldMapBorderStyle.Dotted => DottedStyle,
        _ => SolidStyle
    };

    internal void ResetStrokeCaches()
    {
        foreach (var country in _countries)
        {
            country.CachedStroke?.Dispose();
            country.CachedStroke = null;
            country.CachedRim?.Dispose();
            country.CachedRim = null;
            country.CachedStrokeScale = 0;
        }

        Invalidate();
    }

    private readonly HashSet<string> _selected = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Color> _colors = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, WorldMapCountry> _byCode = new(StringComparer.OrdinalIgnoreCase);

    private CanvasControl? _canvas;
    private Border? _toolTip;
    private TextBlock? _toolTipText;
    private readonly TranslateTransform _toolTipTransform = new();
    private FrameworkElement? _zoomPanel;
    private ButtonBase? _zoomIn;
    private ButtonBase? _zoomOut;
    private ButtonBase? _reset;

    private WorldMapDataSet? _dataSet;
    private List<WorldMapCountry> _countries = new();
    private WorldMapCountry? _hovered;
    private bool _isLoaded;
    private bool _updatingSelection;

    private Rect _content;
    private Vector2 _origin;
    private float _baseScale;
    private float _scale = 1f;
    private float _targetScale = 1f;
    private Vector2 _offset;
    private Vector2 _targetOffset;
    private Matrix3x2 _transform = Matrix3x2.Identity;
    private double _viewWidth;
    private double _viewHeight;

    private bool _pressed;
    private bool _dragging;
    private Point _pressPoint;
    private Vector2 _pressOffset;

    private long _lastHitTest;
    private bool _animating;
    private long _lastTick;

    public WorldMap()
    {
        DefaultStyleKey = typeof(WorldMap);
        Unloaded += OnControlUnloaded;
    }

    public event EventHandler? MapLoaded;
    public event EventHandler<WorldMapCountryEventArgs>? CountryClicked;
    public event EventHandler<WorldMapCountryEventArgs>? HoveredCountryChanged;
    public event EventHandler<WorldMapSelectionChangedEventArgs>? SelectionChanged;
    public event EventHandler? ViewChanged;

    public IReadOnlyList<WorldMapCountry> Countries => _countries;
    public IReadOnlyCollection<string> SelectedCodes => _selected;
    public WorldMapCountry? HoveredCountry => _hovered;
    public bool IsMapLoaded => _isLoaded;
    public double ZoomLevel => _baseScale > 0 ? _scale / _baseScale : 1d;

    public bool TryGetCountry(string code, out WorldMapCountry? country) => _byCode.TryGetValue(code, out country);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        DetachTemplate();

        _canvas = GetTemplateChild(CanvasPartName) as CanvasControl;
        _toolTip = GetTemplateChild(ToolTipPartName) as Border;
        _toolTipText = GetTemplateChild(ToolTipTextPartName) as TextBlock;
        if (_toolTip is not null)
        {
            _toolTip.Margin = new Thickness(0);
            _toolTip.RenderTransform = _toolTipTransform;
        }

        _zoomPanel = GetTemplateChild(ZoomPanelPartName) as FrameworkElement;
        _zoomIn = GetTemplateChild(ZoomInPartName) as ButtonBase;
        _zoomOut = GetTemplateChild(ZoomOutPartName) as ButtonBase;
        _reset = GetTemplateChild(ResetPartName) as ButtonBase;

        if (_canvas is not null)
        {
            _canvas.CreateResources += OnCreateResources;
            _canvas.Draw += OnDraw;
            _canvas.SizeChanged += OnCanvasSizeChanged;
            _canvas.PointerPressed += OnPointerPressed;
            _canvas.PointerMoved += OnPointerMoved;
            _canvas.PointerReleased += OnPointerReleased;
            _canvas.PointerCanceled += OnPointerCanceled;
            _canvas.PointerCaptureLost += OnPointerCaptureLost;
            _canvas.PointerExited += OnPointerExited;
            _canvas.PointerWheelChanged += OnPointerWheelChanged;
        }

        if (_zoomIn is not null) _zoomIn.Click += OnZoomInClick;
        if (_zoomOut is not null) _zoomOut.Click += OnZoomOutClick;
        if (_reset is not null) _reset.Click += OnResetClick;

        ActualThemeChanged -= OnActualThemeChanged;
        ActualThemeChanged += OnActualThemeChanged;

        UpdateZoomControlsVisibility();
    }

    private void DetachTemplate()
    {
        if (_canvas is not null)
        {
            _canvas.CreateResources -= OnCreateResources;
            _canvas.Draw -= OnDraw;
            _canvas.SizeChanged -= OnCanvasSizeChanged;
            _canvas.PointerPressed -= OnPointerPressed;
            _canvas.PointerMoved -= OnPointerMoved;
            _canvas.PointerReleased -= OnPointerReleased;
            _canvas.PointerCanceled -= OnPointerCanceled;
            _canvas.PointerCaptureLost -= OnPointerCaptureLost;
            _canvas.PointerExited -= OnPointerExited;
            _canvas.PointerWheelChanged -= OnPointerWheelChanged;
        }

        if (_zoomIn is not null) _zoomIn.Click -= OnZoomInClick;
        if (_zoomOut is not null) _zoomOut.Click -= OnZoomOutClick;
        if (_reset is not null) _reset.Click -= OnResetClick;
    }

    private void OnControlUnloaded(object sender, RoutedEventArgs e)
    {
        StopAnimation();
        ClearBrushes();
        _grid?.Dispose();
        _grid = null;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        Invalidate();
    }

    internal void UpdateZoomControlsVisibility()
    {
        if (_zoomPanel is not null)
        {
            _zoomPanel.Visibility = ShowZoomControls ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    internal void Invalidate()
    {
        _canvas?.Invalidate();
    }

    private void OnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
    }

    private async Task CreateResourcesAsync(CanvasControl sender)
    {
        _grid?.Dispose();
        _grid = null;
        ClearBrushes();
        _dataSet ??= await WorldMapData.LoadAsync(DataSource);

        var device = sender.Device;
        var countries = _dataSet.Countries;

        await Task.Run(() =>
        {
            Parallel.ForEach(countries, country =>
            {
                country.CachedStroke = null;
                country.CachedStrokeScale = 0;
                country.CachedRim = null;
                country.CachedFill = null;
                country.Geometry?.Dispose();
                country.Geometry = BuildGeometry(device, country);
                country.Bounds = country.Geometry.ComputeBounds();
            });
        });

        _countries = countries;
        _byCode.Clear();
        foreach (var country in countries)
        {
            _byCode[country.Code] = country;
        }

        double left = double.MaxValue, top = double.MaxValue, right = double.MinValue, bottom = double.MinValue;
        foreach (var country in countries)
        {
            left = Math.Min(left, country.Bounds.Left);
            top = Math.Min(top, country.Bounds.Top);
            right = Math.Max(right, country.Bounds.Right);
            bottom = Math.Max(bottom, country.Bounds.Bottom);
        }

        _content = new Rect(left - ContentPadding, top - ContentPadding, right - left + ContentPadding * 2, bottom - top + ContentPadding * 2);
        _origin = new Vector2((float)_content.X, (float)_content.Y);

        _viewWidth = sender.ActualWidth;
        _viewHeight = sender.ActualHeight;
        _baseScale = ComputeBaseScale();

        _isLoaded = true;
        ResetView(false);
        sender.Invalidate();
        MapLoaded?.Invoke(this, EventArgs.Empty);
    }

    private static CanvasGeometry BuildGeometry(Microsoft.Graphics.Canvas.ICanvasResourceCreator device, WorldMapCountry country)
    {
        var parts = new List<CanvasGeometry>(country.Shapes.Count);

        foreach (var shape in country.Shapes)
        {
            using var builder = new CanvasPathBuilder(device);
            builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Alternate);

            foreach (var figure in shape.Figures)
            {
                AddFigure(builder, figure);
            }

            parts.Add(CanvasGeometry.CreatePath(builder));
        }

        if (parts.Count == 1)
        {
            return parts[0];
        }

        var group = CanvasGeometry.CreateGroup(device, parts.ToArray(), CanvasFilledRegionDetermination.Winding);
        foreach (var part in parts)
        {
            part.Dispose();
        }

        return group;
    }

    private static void AddFigure(CanvasPathBuilder builder, WorldMapFigureData figure)
    {
        var ops = figure.Ops;
        var pts = figure.Points;
        int p = 0;
        bool open = false;

        for (int i = 0; i < ops.Length; i++)
        {
            switch (ops[i])
            {
                case 0:
                    if (open)
                    {
                        builder.EndFigure(CanvasFigureLoop.Closed);
                    }

                    builder.BeginFigure(pts[p], pts[p + 1]);
                    p += 2;
                    open = true;
                    break;
                case 1:
                    builder.AddLine(pts[p], pts[p + 1]);
                    p += 2;
                    break;
                case 2:
                    builder.AddCubicBezier(
                        new Vector2(pts[p], pts[p + 1]),
                        new Vector2(pts[p + 2], pts[p + 3]),
                        new Vector2(pts[p + 4], pts[p + 5]));
                    p += 6;
                    break;
                default:
                    builder.EndFigure(CanvasFigureLoop.Closed);
                    open = false;
                    break;
            }
        }

        if (open)
        {
            builder.EndFigure(CanvasFigureLoop.Closed);
        }
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        var session = args.DrawingSession;
        DrawGrid(sender, session);
        session.Transform = _transform;

        var visible = GetVisibleMapRect();
        float strokeWidth = (float)(CountryStrokeThickness / _scale);
        bool dark = ActualTheme == ElementTheme.Dark;

        var fill = ResolveColor(CountryFill, dark ? Color.FromArgb(255, 58, 64, 73) : Color.FromArgb(255, 201, 208, 217));
        var hoverFill = ResolveColor(CountryHoverFill, dark ? Color.FromArgb(255, 92, 100, 112) : Color.FromArgb(255, 160, 171, 186));
        var selectedFill = ResolveColor(CountrySelectedFill, Color.FromArgb(255, 0, 120, 212));
        var stroke = ResolveColor(CountryStroke, dark ? Color.FromArgb(255, 27, 31, 36) : Colors.White);

        if (TryGetPresetColor(BorderColor, CustomBorderColor, out var borderPreset))
        {
            stroke = borderPreset;
        }

        if (TryGetPresetColor(CountryColor, CustomCountryColor, out var normalPreset))
        {
            fill = normalPreset;
        }

        if (TryGetPresetColor(HighlightColor, CustomHighlightColor, out var preset))
        {
            selectedFill = preset;
            hoverFill = Blend(preset, Colors.White, 0.15f);
        }

        if (TryGetPresetColor(SelectedColor, CustomSelectedColor, out var selectedPreset))
        {
            selectedFill = selectedPreset;
        }

        EnsureCaches(visible);
        var defaultBrush = GetGradientBrush(session, WithAlpha(Blend(fill, Colors.White, 0.25f), 240), WithAlpha(fill, 55));
        WorldMapCountry? hoveredCountry = null;
        var selectedList = new List<WorldMapCountry>();

        foreach (var country in _countries)
        {
            if (country.Geometry is null || !Intersects(country.Bounds, visible))
            {
                continue;
            }

            if (_selected.Contains(country.Code))
            {
                selectedList.Add(country);
                continue;
            }

            if (ReferenceEquals(country, _hovered))
            {
                hoveredCountry = country;
                continue;
            }

            {
                // Brushes are cached per color and re-aimed per country; heat map colors get the same gradient.
                var cb = country.Bounds;
                var fillBrush = _colors.TryGetValue(country.Code, out var custom)
                    ? GetGradientBrush(session, WithAlpha(Blend(custom, Colors.White, 0.2f), custom.A), WithAlpha(custom, (byte)(custom.A * 0.6)))
                    : defaultBrush;

                fillBrush.StartPoint = new Vector2(0, (float)cb.Y);
                fillBrush.EndPoint = new Vector2(0, (float)(cb.Y + cb.Height));
                if (country.CachedFill is not null)
                {
                    session.DrawCachedGeometry(country.CachedFill, fillBrush);
                }
                else
                {
                    session.FillGeometry(country.Geometry, fillBrush);
                }
            }

            if (strokeWidth > 0 && CountryBorderStyle != WorldMapBorderStyle.None)
            {
                if (country.CachedStroke is not null)
                {
                    session.DrawCachedGeometry(country.CachedStroke, stroke);
                }
            }
        }

        // Highlighted countries are drawn last with a translucent gradient, glow and bright rim.
        if (hoveredCountry is not null)
        {
            var color = _colors.TryGetValue(hoveredCountry.Code, out var custom) ? Blend(custom, Colors.White, 0.25f) : hoverFill;
            DrawHighlighted(session, hoveredCountry, color, 0.6f);
        }

        foreach (var country in selectedList)
        {
            DrawHighlighted(session, country, selectedFill, 1f);
        }
    }

    private static void DrawCachedFill(Microsoft.Graphics.Canvas.CanvasDrawingSession session, WorldMapCountry country, Color color)
    {
        if (country.CachedFill is not null)
        {
            session.DrawCachedGeometry(country.CachedFill, color);
        }
        else
        {
            session.FillGeometry(country.Geometry, color);
        }
    }

    private bool _fillBuilding;
    private bool _strokeBuilding;

    // Cached geometry lives within a zoom band; border width only drifts a little, so rebuilds are rare.
    private static bool IsScaleCompatible(float builtScale, float scale)
    {
        if (builtScale <= 0)
        {
            return false;
        }

        float ratio = scale / builtScale;
        return ratio is > 0.625f and < 1.6f;
    }

    private void EnsureCaches(Rect visible)
    {
        if (!_fillBuilding && _countries.Any(c => c.Geometry is not null && c.CachedFill is null))
        {
            _ = BuildFillsAsync();
        }

        if (_strokeBuilding || _animating)
        {
            return;
        }

        var style = CountryBorderStyle;
        float px = style == WorldMapBorderStyle.None ? 0f : (float)CountryStrokeThickness;
        if (px <= 0)
        {
            return;
        }

        List<WorldMapCountry>? needed = null;
        foreach (var country in _countries)
        {
            if (country.Geometry is null || !Intersects(country.Bounds, visible))
            {
                continue;
            }

            if (country.CachedStrokeStyle != style
                || Math.Abs(country.CachedStrokePx - px) > 0.01f
                || !IsScaleCompatible(country.CachedStrokeScale, _scale))
            {
                (needed ??= new()).Add(country);
            }
        }

        if (needed is not null)
        {
            _ = BuildStrokesAsync(needed, px, style);
        }
    }

    // Tessellation runs off the UI thread; drawing falls back to live geometry until the caches are swapped in.
    private async Task BuildFillsAsync()
    {
        var device = _canvas?.Device;
        if (device is null)
        {
            return;
        }

        _fillBuilding = true;
        var countries = _countries;
        float fillTolerance = 0.5f / (Math.Max(_baseScale, 0.0001f) * 4f);
        var fills = new Microsoft.Graphics.Canvas.Geometry.CanvasCachedGeometry?[countries.Count];

        try
        {
            await Task.Run(() => Parallel.For(0, countries.Count, i =>
            {
                var geometry = countries[i].Geometry;
                if (geometry is not null && countries[i].CachedFill is null)
                {
                    fills[i] = Microsoft.Graphics.Canvas.Geometry.CanvasCachedGeometry.CreateFill(geometry, fillTolerance);
                }
            }));
        }
        catch (Exception)
        {
            // Device may have been lost mid-build.
        }

        for (int i = 0; i < countries.Count; i++)
        {
            if (fills[i] is null)
            {
                continue;
            }

            if (ReferenceEquals(countries, _countries))
            {
                countries[i].CachedFill?.Dispose();
                countries[i].CachedFill = fills[i];
            }
            else
            {
                fills[i]!.Dispose();
            }
        }

        _fillBuilding = false;
        Invalidate();
    }

    // Borders are stroked from a simplified outline sized for the current zoom, which is far cheaper than stroking the full data.
    private async Task BuildStrokesAsync(List<WorldMapCountry> needed, float px, WorldMapBorderStyle style)
    {
        var device = _canvas?.Device;
        if (device is null)
        {
            return;
        }

        _strokeBuilding = true;
        var countries = _countries;
        float scale = _scale;
        var strokeStyle = GetStrokeStyle(style);
        var results = new Microsoft.Graphics.Canvas.Geometry.CanvasCachedGeometry?[needed.Count];

        try
        {
            await Task.Run(() => Parallel.For(0, needed.Count, i =>
            {
                results[i] = CreateBorder(device, needed[i], px, scale, strokeStyle);
            }));
        }
        catch (Exception)
        {
            // Device may have been lost mid-build.
        }

        for (int i = 0; i < needed.Count; i++)
        {
            if (ReferenceEquals(countries, _countries))
            {
                // A null result is still recorded so a country without an outline is not retried every frame.
                var country = needed[i];
                country.CachedStroke?.Dispose();
                country.CachedStroke = results[i];
                country.CachedStrokeScale = scale;
                country.CachedStrokePx = px;
                country.CachedStrokeStyle = style;
            }
            else
            {
                results[i]?.Dispose();
            }
        }

        _strokeBuilding = false;
        Invalidate();
    }

    private static Microsoft.Graphics.Canvas.Geometry.CanvasCachedGeometry? CreateBorder(
        Microsoft.Graphics.Canvas.ICanvasResourceCreator device, WorldMapCountry country, float px, float scale, CanvasStrokeStyle strokeStyle)
    {
        using var outline = BuildSimplifiedOutline(device, country, 0.4f / scale);
        return outline is null
            ? null
            : Microsoft.Graphics.Canvas.Geometry.CanvasCachedGeometry.CreateStroke(outline, px / scale, strokeStyle, 0.5f / scale);
    }

    private static CanvasGeometry? BuildSimplifiedOutline(Microsoft.Graphics.Canvas.ICanvasResourceCreator device, WorldMapCountry country, float tolerance)
    {
        using var builder = new CanvasPathBuilder(device);
        var line = new List<Vector2>(1024);
        bool any = false;

        void Flush()
        {
            if (line.Count < 2)
            {
                line.Clear();
                return;
            }

            var keep = SimplifyPolyline(line, tolerance);
            bool begun = false;
            for (int i = 0; i < line.Count; i++)
            {
                if (!keep[i])
                {
                    continue;
                }

                if (!begun)
                {
                    builder.BeginFigure(line[i]);
                    begun = true;
                }
                else
                {
                    builder.AddLine(line[i]);
                }
            }

            if (begun)
            {
                builder.EndFigure(CanvasFigureLoop.Closed);
                any = true;
            }

            line.Clear();
        }

        foreach (var shape in country.Shapes)
        {
            foreach (var figure in shape.Figures)
            {
                var ops = figure.Ops;
                var pts = figure.Points;
                int p = 0;

                for (int i = 0; i < ops.Length; i++)
                {
                    switch (ops[i])
                    {
                        case 0:
                            Flush();
                            line.Add(new Vector2(pts[p], pts[p + 1]));
                            p += 2;
                            break;
                        case 1:
                            line.Add(new Vector2(pts[p], pts[p + 1]));
                            p += 2;
                            break;
                        case 2:
                            {
                                var p0 = line.Count > 0 ? line[^1] : new Vector2(pts[p], pts[p + 1]);
                                var c1 = new Vector2(pts[p], pts[p + 1]);
                                var c2 = new Vector2(pts[p + 2], pts[p + 3]);
                                var p3 = new Vector2(pts[p + 4], pts[p + 5]);
                                for (int k = 1; k <= 4; k++)
                                {
                                    float tt = k / 4f, u = 1 - tt;
                                    line.Add(u * u * u * p0 + 3 * u * u * tt * c1 + 3 * u * tt * tt * c2 + tt * tt * tt * p3);
                                }

                                p += 6;
                                break;
                            }
                        default:
                            Flush();
                            break;
                    }
                }

                Flush();
            }
        }

        return any ? CanvasGeometry.CreatePath(builder) : null;
    }

    // Iterative Douglas-Peucker; returns which points to keep.
    private static bool[] SimplifyPolyline(List<Vector2> points, float tolerance)
    {
        int n = points.Count;
        var keep = new bool[n];
        keep[0] = true;
        keep[n - 1] = true;

        var stack = new Stack<(int, int)>();
        stack.Push((0, n - 1));
        float tol2 = tolerance * tolerance;

        while (stack.Count > 0)
        {
            var (a, b) = stack.Pop();
            if (b <= a + 1)
            {
                continue;
            }

            var pa = points[a];
            var ab = points[b] - pa;
            float len2 = ab.LengthSquared();
            float maxD = -1f;
            int idx = -1;

            for (int i = a + 1; i < b; i++)
            {
                var ap = points[i] - pa;
                float d2;
                if (len2 < 1e-12f)
                {
                    d2 = ap.LengthSquared();
                }
                else
                {
                    float cross = ab.X * ap.Y - ab.Y * ap.X;
                    d2 = cross * cross / len2;
                }

                if (d2 > maxD)
                {
                    maxD = d2;
                    idx = i;
                }
            }

            if (maxD > tol2)
            {
                keep[idx] = true;
                stack.Push((a, idx));
                stack.Push((idx, b));
            }
        }

        return keep;
    }
    private void DrawHighlighted(Microsoft.Graphics.Canvas.CanvasDrawingSession session, WorldMapCountry country, Color color, float intensity)
    {
        var b = country.Bounds;
        var rim = Blend(color, Colors.White, 0.35f);

        // Vertical gradient: vivid at the top, fading to transparent at the bottom.
        var top = Color.FromArgb((byte)(235 * intensity), color.R, color.G, color.B);
        var bottom = Color.FromArgb((byte)(28 + 30 * intensity), color.R, color.G, color.B);

        using var brush = new Microsoft.Graphics.Canvas.Brushes.CanvasLinearGradientBrush(session, top, bottom)
        {
            StartPoint = new Vector2(0, (float)b.Y),
            EndPoint = new Vector2(0, (float)(b.Y + b.Height)),
        };

        if (country.CachedFill is not null)
        {
            session.DrawCachedGeometry(country.CachedFill, brush);
        }
        else
        {
            session.FillGeometry(country.Geometry, brush);
        }

        float rimPx = Math.Max((float)CountryStrokeThickness * 1.6f, 1.2f);
        bool rimStale = country.CachedRim is null
            || Math.Abs(country.CachedRimPx - rimPx) > 0.01f
            || !IsScaleCompatible(country.CachedRimScale, _scale);
        if (rimStale)
        {
            country.CachedRim?.Dispose();
            country.CachedRim = CreateBorder(_canvas!, country, rimPx, _scale, GetStrokeStyle(CountryBorderStyle));
            country.CachedRimScale = _scale;
            country.CachedRimPx = rimPx;
        }

        session.DrawCachedGeometry(country.CachedRim, rim);
    }

    private Microsoft.Graphics.Canvas.CanvasRenderTarget? _grid;
    private bool _gridDark;

    // The dot grid is rendered once into a bitmap and blitted each frame.
    private void DrawGrid(CanvasControl sender, Microsoft.Graphics.Canvas.CanvasDrawingSession session)
    {
        double width = sender.ActualWidth, height = sender.ActualHeight;
        if (width < 1 || height < 1)
        {
            return;
        }

        bool dark = ActualTheme == ElementTheme.Dark;
        if (_grid is null || _gridDark != dark || Math.Abs(_grid.Size.Width - width) > 0.5 || Math.Abs(_grid.Size.Height - height) > 0.5)
        {
            _grid?.Dispose();
            _grid = new Microsoft.Graphics.Canvas.CanvasRenderTarget(sender, (float)width, (float)height);
            _gridDark = dark;

            var dot = dark ? Color.FromArgb(34, 255, 255, 255) : Color.FromArgb(40, 60, 90, 130);
            using var gs = _grid.CreateDrawingSession();
            gs.Clear(Colors.Transparent);
            const float step = 14f;
            for (float y = step / 2; y < height; y += step)
            {
                for (float x = step / 2; x < width; x += step)
                {
                    gs.FillCircle(x, y, 1f, dot);
                }
            }
        }

        session.DrawImage(_grid);
    }
    private static bool TryGetPresetColor(WorldMapHighlightColor kind, Color custom, out Color color)
    {
        color = kind switch
        {
            WorldMapHighlightColor.Red => Color.FromArgb(255, 239, 68, 68),
            WorldMapHighlightColor.Orange => Color.FromArgb(255, 249, 115, 22),
            WorldMapHighlightColor.Yellow => Color.FromArgb(255, 250, 204, 21),
            WorldMapHighlightColor.Green => Color.FromArgb(255, 34, 197, 94),
            WorldMapHighlightColor.Cyan => Color.FromArgb(255, 6, 182, 212),
            WorldMapHighlightColor.Blue => Color.FromArgb(255, 59, 130, 246),
            WorldMapHighlightColor.Purple => Color.FromArgb(255, 168, 85, 247),
            WorldMapHighlightColor.Pink => Color.FromArgb(255, 236, 72, 153),
            WorldMapHighlightColor.White => Color.FromArgb(255, 241, 245, 249),
            WorldMapHighlightColor.Custom => custom,
            _ => default
        };

        return kind != WorldMapHighlightColor.Default;
    }

    private readonly Dictionary<(Color, Color), Microsoft.Graphics.Canvas.Brushes.CanvasLinearGradientBrush> _brushes = new();

    private Microsoft.Graphics.Canvas.Brushes.CanvasLinearGradientBrush GetGradientBrush(Microsoft.Graphics.Canvas.ICanvasResourceCreator device, Color top, Color bottom)
    {
        if (_brushes.TryGetValue((top, bottom), out var brush))
        {
            return brush;
        }

        if (_brushes.Count > 300)
        {
            ClearBrushes();
        }

        brush = new Microsoft.Graphics.Canvas.Brushes.CanvasLinearGradientBrush(device, top, bottom);
        _brushes[(top, bottom)] = brush;
        return brush;
    }

    private void ClearBrushes()
    {
        foreach (var brush in _brushes.Values)
        {
            brush.Dispose();
        }

        _brushes.Clear();
    }

    private static Color WithAlpha(Color c, byte a) => Color.FromArgb(a, c.R, c.G, c.B);

    private static bool Intersects(Rect a, Rect b)
    {
        return a.X < b.X + b.Width && a.X + a.Width > b.X && a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;
    }

    private static Color ResolveColor(Brush? brush, Color fallback)
    {
        if (brush is SolidColorBrush solid)
        {
            var c = solid.Color;
            return Color.FromArgb((byte)Math.Round(c.A * solid.Opacity), c.R, c.G, c.B);
        }

        return fallback;
    }

    private static Color Blend(Color a, Color b, float t)
    {
        return Color.FromArgb(
            (byte)(a.A + (b.A - a.A) * t),
            (byte)(a.R + (b.R - a.R) * t),
            (byte)(a.G + (b.G - a.G) * t),
            (byte)(a.B + (b.B - a.B) * t));
    }

    public void SetCountryColor(string code, Color color)
    {
        _colors[code] = color;
        Invalidate();
    }

    public void SetCountryColors(IEnumerable<KeyValuePair<string, Color>> colors)
    {
        foreach (var pair in colors)
        {
            _colors[pair.Key] = pair.Value;
        }

        Invalidate();
    }

    public void SetCountryValues(IReadOnlyDictionary<string, double> values, Color low, Color high)
    {
        _colors.Clear();

        if (values.Count > 0)
        {
            double min = values.Values.Min();
            double span = values.Values.Max() - min;

            foreach (var pair in values)
            {
                float t = span <= 0 ? 1f : (float)((pair.Value - min) / span);
                _colors[pair.Key] = Blend(low, high, t);
            }
        }

        Invalidate();
    }

    public void ClearCountryColors()
    {
        _colors.Clear();
        Invalidate();
    }

    public void SelectCountry(string code)
    {
        if (SelectionMode == WorldMapSelectionMode.None || !_byCode.TryGetValue(code, out var country))
        {
            return;
        }

        if (SelectionMode == WorldMapSelectionMode.Single)
        {
            SetSelection(new[] { country.Code });
        }
        else if (!_selected.Contains(country.Code))
        {
            SetSelection(_selected.Append(country.Code));
        }
    }

    public void DeselectCountry(string code)
    {
        if (_selected.Contains(code))
        {
            SetSelection(_selected.Where(c => !string.Equals(c, code, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public void ClearSelection()
    {
        if (_selected.Count > 0)
        {
            SetSelection(Array.Empty<string>());
        }
    }

    private void SetSelection(IEnumerable<string> codes)
    {
        var next = codes.ToList();
        var removed = _selected.Where(c => !next.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
        var added = next.Where(c => !_selected.Contains(c)).ToList();

        if (removed.Count == 0 && added.Count == 0)
        {
            return;
        }

        _selected.Clear();
        foreach (var code in next)
        {
            _selected.Add(code);
        }

        _updatingSelection = true;
        SelectedCountryCode = next.Count > 0 ? next[^1] : null;
        _updatingSelection = false;

        Invalidate();
        SelectionChanged?.Invoke(this, new WorldMapSelectionChangedEventArgs(added, removed));
    }

    private void ToggleSelection(WorldMapCountry country)
    {
        switch (SelectionMode)
        {
            case WorldMapSelectionMode.Single:
                SetSelection(new[] { country.Code });
                break;
            case WorldMapSelectionMode.Multiple:
                if (_selected.Contains(country.Code))
                {
                    DeselectCountry(country.Code);
                }
                else
                {
                    SelectCountry(country.Code);
                }

                break;
        }
    }

    private float ComputeBaseScale()
    {
        if (_content.Width <= 0 || _content.Height <= 0 || _viewWidth <= 0 || _viewHeight <= 0)
        {
            return 0f;
        }

        return (float)Math.Min(_viewWidth / _content.Width, _viewHeight / _content.Height);
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _viewWidth = e.NewSize.Width;
        _viewHeight = e.NewSize.Height;

        if (!_isLoaded)
        {
            return;
        }

        float oldBase = _baseScale;
        float zoom = oldBase > 0 ? _targetScale / oldBase : 1f;
        _baseScale = ComputeBaseScale();

        if (_baseScale <= 0)
        {
            return;
        }

        float scale = _baseScale * zoom;
        SetTarget(scale, _targetOffset, false);
    }

    private Vector2 ScreenToMap(Vector2 screen)
    {
        return (screen - _offset) / _scale + _origin;
    }

    private Rect GetVisibleMapRect()
    {
        var topLeft = ScreenToMap(Vector2.Zero);
        var bottomRight = ScreenToMap(new Vector2((float)_viewWidth, (float)_viewHeight));
        return new Rect(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }

    private Vector2 ClampOffset(Vector2 offset, float scale)
    {
        float contentWidth = (float)_content.Width * scale;
        float contentHeight = (float)_content.Height * scale;
        float viewWidth = (float)_viewWidth;
        float viewHeight = (float)_viewHeight;

        offset.X = contentWidth <= viewWidth ? (viewWidth - contentWidth) / 2f : Math.Clamp(offset.X, viewWidth - contentWidth, 0f);
        offset.Y = contentHeight <= viewHeight ? (viewHeight - contentHeight) / 2f : Math.Clamp(offset.Y, viewHeight - contentHeight, 0f);
        return offset;
    }

    private void UpdateTransform()
    {
        _transform = Matrix3x2.CreateTranslation(-_origin) * Matrix3x2.CreateScale(_scale) * Matrix3x2.CreateTranslation(_offset);
    }

    private void SetTarget(float scale, Vector2 offset, bool animate)
    {
        if (_baseScale <= 0)
        {
            return;
        }

        scale = Math.Clamp(scale, _baseScale * (float)Math.Max(MinZoom, 0.01), _baseScale * (float)Math.Max(MaxZoom, MinZoom));
        _targetScale = scale;
        _targetOffset = ClampOffset(offset, scale);

        if (animate)
        {
            StartAnimation();
            return;
        }

        StopAnimation();
        _scale = _targetScale;
        _offset = _targetOffset;
        UpdateTransform();
        Invalidate();
        ViewChanged?.Invoke(this, EventArgs.Empty);
    }

    private void StartAnimation()
    {
        if (_animating)
        {
            return;
        }

        _animating = true;
        _lastTick = Stopwatch.GetTimestamp();
        CompositionTarget.Rendering += OnRendering;
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
        long now = Stopwatch.GetTimestamp();
        double dt = (now - _lastTick) / (double)Stopwatch.Frequency;
        _lastTick = now;

        float k = (float)(1 - Math.Exp(-dt * 14));
        _scale += (_targetScale - _scale) * k;
        _offset += (_targetOffset - _offset) * k;

        bool done = Math.Abs(_targetScale - _scale) < _targetScale * 0.0005f && Vector2.Distance(_offset, _targetOffset) < 0.3f;
        if (done)
        {
            _scale = _targetScale;
            _offset = _targetOffset;
            StopAnimation();
        }

        UpdateTransform();
        Invalidate();
        ViewChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ResetView(bool animate = true)
    {
        SetTarget(_baseScale, Vector2.Zero, animate);
    }

    public void ZoomIn() => ZoomBy(1.6f, new Vector2((float)_viewWidth / 2f, (float)_viewHeight / 2f));

    public void ZoomOut() => ZoomBy(1f / 1.6f, new Vector2((float)_viewWidth / 2f, (float)_viewHeight / 2f));

    public void ZoomBy(float factor, Vector2 screenPoint, bool animate = true)
    {
        if (!_isLoaded)
        {
            return;
        }

        float scale = Math.Clamp(_targetScale * factor, _baseScale * (float)MinZoom, _baseScale * (float)MaxZoom);
        var mapPoint = (screenPoint - _targetOffset) / _targetScale + _origin;
        var offset = screenPoint - (mapPoint - _origin) * scale;
        SetTarget(scale, offset, animate);
    }

    public bool ZoomToCountry(string code, bool animate = true)
    {
        if (!_isLoaded || !_byCode.TryGetValue(code, out var country))
        {
            return false;
        }

        var bounds = country.MainBounds;
        double width = Math.Max(bounds.Width, 8) * 1.5;
        double height = Math.Max(bounds.Height, 8) * 1.5;
        float scale = (float)Math.Min(_viewWidth / width, _viewHeight / height);

        var center = new Vector2((float)(bounds.X + bounds.Width / 2), (float)(bounds.Y + bounds.Height / 2));
        var viewCenter = new Vector2((float)_viewWidth / 2f, (float)_viewHeight / 2f);
        var offset = viewCenter - (center - _origin) * Math.Clamp(scale, _baseScale * (float)MinZoom, _baseScale * (float)MaxZoom);

        SetTarget(scale, offset, animate);
        return true;
    }

    private WorldMapCountry? HitTest(Vector2 screen)
    {
        if (!_isLoaded)
        {
            return null;
        }

        var map = ScreenToMap(screen);

        for (int i = _countries.Count - 1; i >= 0; i--)
        {
            var country = _countries[i];
            var b = country.Bounds;

            if (country.Geometry is null || map.X < b.X || map.X > b.X + b.Width || map.Y < b.Y || map.Y > b.Y + b.Height)
            {
                continue;
            }

            if (country.Geometry.FillContainsPoint(map))
            {
                return country;
            }
        }

        return null;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(_canvas);
        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _pressed = true;
        _dragging = false;
        _pressPoint = point.Position;
        _pressOffset = _offset;
        _canvas?.CapturePointer(e.Pointer);
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var position = e.GetCurrentPoint(_canvas).Position;

        if (_pressed && IsPanEnabled)
        {
            double dx = position.X - _pressPoint.X;
            double dy = position.Y - _pressPoint.Y;

            if (!_dragging && Math.Sqrt(dx * dx + dy * dy) > DragThreshold)
            {
                _dragging = true;
                SetHovered(null, position);
            }

            if (_dragging)
            {
                var offset = ClampOffset(_pressOffset + new Vector2((float)dx, (float)dy), _scale);
                StopAnimation();
                _offset = offset;
                _targetOffset = offset;
                _targetScale = _scale;
                UpdateTransform();
                Invalidate();
                ViewChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        if (!_dragging)
        {
            long now = Stopwatch.GetTimestamp();
            if ((now - _lastHitTest) * 1000 < Stopwatch.Frequency * 24)
            {
                UpdateToolTip(position);
                return;
            }

            _lastHitTest = now;
            SetHovered(HitTest(new Vector2((float)position.X, (float)position.Y)), position);
        }
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        bool wasDragging = _dragging;
        _pressed = false;
        _dragging = false;
        _canvas?.ReleasePointerCapture(e.Pointer);

        if (wasDragging)
        {
            return;
        }

        var position = e.GetCurrentPoint(_canvas).Position;
        var country = HitTest(new Vector2((float)position.X, (float)position.Y));

        if (country is not null)
        {
            ToggleSelection(country);
        }
        else if (ClearSelectionOnEmptyClick)
        {
            ClearSelection();
        }

        CountryClicked?.Invoke(this, new WorldMapCountryEventArgs(country));
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
    {
        _pressed = false;
        _dragging = false;
    }

    private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
    {
        _pressed = false;
        _dragging = false;
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (!_pressed)
        {
            SetHovered(null, default);
        }
    }

    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (!IsZoomEnabled || !_isLoaded)
        {
            return;
        }

        var point = e.GetCurrentPoint(_canvas);
        float factor = (float)Math.Pow(1.0015, point.Properties.MouseWheelDelta);
        ZoomBy(factor, new Vector2((float)point.Position.X, (float)point.Position.Y));
        e.Handled = true;
    }

    private void SetHovered(WorldMapCountry? country, Point position)
    {
        if (!ReferenceEquals(country, _hovered))
        {
            _hovered = country;
            Invalidate();
            HoveredCountryChanged?.Invoke(this, new WorldMapCountryEventArgs(country));
        }

        UpdateToolTip(position);
    }

    private void UpdateToolTip(Point position)
    {
        if (_toolTip is null || _toolTipText is null)
        {
            return;
        }

        if (!ShowToolTip || _hovered is null)
        {
            _toolTip.Visibility = Visibility.Collapsed;
            return;
        }

        string text = ToolTipFormatter?.Invoke(_hovered) ?? _hovered.Name;
        if (!string.Equals(_toolTipText.Text, text, StringComparison.Ordinal))
        {
            _toolTipText.Text = text;
            _toolTip.InvalidateMeasure();
        }

        _toolTip.Visibility = Visibility.Visible;
        _toolTip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        var size = _toolTip.DesiredSize;
        double x = position.X + 16;
        double y = position.Y + 20;

        if (x + size.Width > _viewWidth)
        {
            x = position.X - size.Width - 8;
        }

        if (y + size.Height > _viewHeight)
        {
            y = position.Y - size.Height - 8;
        }

        _toolTipTransform.X = Math.Max(0, x);
        _toolTipTransform.Y = Math.Max(0, y);
    }

    private void OnZoomInClick(object sender, RoutedEventArgs e) => ZoomIn();

    private void OnZoomOutClick(object sender, RoutedEventArgs e) => ZoomOut();

    private void OnResetClick(object sender, RoutedEventArgs e) => ResetView();
}
