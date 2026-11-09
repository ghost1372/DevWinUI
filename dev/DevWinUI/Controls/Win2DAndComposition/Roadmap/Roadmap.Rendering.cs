namespace DevWinUI;

public partial class Roadmap
{
    private const float CapRadius = 4.5f;
    private const float PulsePeriodSeconds = 2f;

    private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        RoadmapPanel? panel = HostPanel;
        if (panel is null || panel.Children.Count == 0)
        {
            return;
        }

        CanvasDrawingSession ds = args.DrawingSession;
        RoadmapMetrics m = GetMetrics();

        using CanvasTextFormat labelFormat = CreateTextFormat(
            ResolveFontFamily(FontFamily, "Segoe UI Variable"), (float)m.LabelFontSize, LabelFontWeight);
        using CanvasTextFormat glyphFormat = CreateTextFormat(
            ResolveFontFamily(IconFontFamily, "Segoe Fluent Icons"), (float)(m.MarkerSize * 0.4), Microsoft.UI.Text.FontWeights.Normal);
        using CanvasStrokeStyle dashed = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };

        Color axisColor = ToColor(AxisBrush, Colors.Gray);

        RoadmapRenderState state = new RoadmapRenderState
        {
            Panel = panel,
            Metrics = m,
            Mode = Mode,
            AxisColor = axisColor,
            StemColor = ToColor(StemBrush, Colors.Gray),
            FillColor = ToColor(MarkerFill, Colors.White),
            TextColor = ToColor(Foreground, Colors.Gray),
            LabelFormat = labelFormat,
            GlyphFormat = glyphFormat,
            Dashed = dashed,
            Pulse = AnimateActive ? (float)(_clock.Elapsed.TotalSeconds % PulsePeriodSeconds / PulsePeriodSeconds) : -1f,
            Boundary = GetProgressBoundary(panel),
            Width = (float)sender.ActualWidth,
            Height = (float)sender.ActualHeight,
            Time = (float)_clock.Elapsed.TotalSeconds,
            Flow = AxisFlowEnabled,
            EffectColor = GetEffectColor(panel, axisColor),
            BootHead = float.NaN
        };

        float boot = GetBootProgress();
        if (boot >= 0f)
        {
            state.BootHead = (float)(panel.AxisStartX + boot * (panel.AxisEndX - panel.AxisStartX));
        }

        bool booting = !float.IsNaN(state.BootHead);
        float flicker = FlickerEnabled ? GetFlickerAlpha(state.Time) : 1f;
        bool glow = ResolvedGlowRadius > 0;

        DrawBackdrop(ds, state);

        using (CanvasActiveLayer? flickerLayer = flicker < 1f ? ds.CreateLayer(flicker) : null)
        {
            using (CanvasActiveLayer? clipLayer = booting ? ds.CreateLayer(1f, new Rect(0, 0, state.BootHead, state.Height)) : null)
            {
                if (glow)
                {
                    DrawGlowed(ds, state);
                }
                else
                {
                    DrawScene(ds, state);
                }
            }

            if (booting)
            {
                DrawBootHead(ds, state);
            }
        }
    }

    private void DrawBackdrop(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        if (GridEnabled)
        {
            Color grid = Color.FromArgb(14, s.TextColor.R, s.TextColor.G, s.TextColor.B);

            for (float x = 0; x < s.Width; x += 28f)
            {
                ds.DrawLine(x, 0, x, s.Height, grid, 1f);
            }

            for (float y = 0; y < s.Height; y += 28f)
            {
                ds.DrawLine(0, y, s.Width, y, grid, 1f);
            }
        }

        if (ScanlinesEnabled)
        {
            Color scan = Color.FromArgb(18, s.TextColor.R, s.TextColor.G, s.TextColor.B);

            for (float y = 0; y < s.Height; y += 4f)
            {
                ds.DrawLine(0, y, s.Width, y, scan, 1f);
            }
        }

        if (ScanBeamEnabled)
        {
            DrawScanBeam(ds, s);
        }

        if (s.Mode == RoadmapMode.Neon)
        {
            Color panelFill = Color.FromArgb(150, s.FillColor.R, s.FillColor.G, s.FillColor.B);

            foreach (UIElement child in s.Panel.Children)
            {
                if (child is RoadmapItem item)
                {
                    ds.FillRectangle(Inflate(item.LayoutBounds, 3), panelFill);
                }
            }
        }
    }

    private static void DrawScanBeam(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        float width = Math.Max(60f, s.Width * 0.12f);
        float period = 5f;
        float phase = s.Time % period / period;
        float x = -width + phase * (s.Width + width * 2f);
        Color c = s.EffectColor;

        CanvasGradientStop[] stops =
        {
            new CanvasGradientStop { Position = 0f, Color = Color.FromArgb(0, c.R, c.G, c.B) },
            new CanvasGradientStop { Position = 0.85f, Color = Color.FromArgb(50, c.R, c.G, c.B) },
            new CanvasGradientStop { Position = 1f, Color = Color.FromArgb(130, c.R, c.G, c.B) }
        };

        using CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(ds, stops)
        {
            StartPoint = new Vector2(x, 0),
            EndPoint = new Vector2(x + width, 0)
        };

        ds.FillRectangle(x, 0, width, s.Height, brush);
        ds.DrawLine(x + width, 0, x + width, s.Height, Color.FromArgb(150, c.R, c.G, c.B), 1.5f);
    }

    private void DrawGlowed(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        using CanvasCommandList glow = new CanvasCommandList(ds);
        using (CanvasDrawingSession glowSession = glow.CreateDrawingSession())
        {
            DrawScene(glowSession, s);
        }

        using GaussianBlurEffect blur = new GaussianBlurEffect
        {
            Source = glow,
            BlurAmount = (float)ResolvedGlowRadius,
            BorderMode = EffectBorderMode.Soft
        };

        ds.DrawImage(blur);
        ds.DrawImage(blur);
        ds.DrawImage(glow);
    }

    private static void DrawBootHead(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        float x = s.BootHead;
        float y = (float)s.Panel.AxisY;
        Color c = Lighten(s.EffectColor);

        ds.DrawLine(x, y - 40f, x, y + 40f, Color.FromArgb(160, c.R, c.G, c.B), 1.5f);
        ds.FillCircle(x, y, 4f, c);
    }

    private static void DrawAxisFlow(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        RoadmapPanel panel = s.Panel;
        float y = (float)panel.AxisY;
        float start = (float)panel.AxisStartX;
        float end = (float)panel.AxisEndX - 18f;

        if (!float.IsNaN(s.Boundary))
        {
            end = Math.Min(end, s.Boundary);
        }

        float length = end - start;
        if (length < 60f)
        {
            return;
        }

        float thickness = (float)s.Metrics.AxisThickness;
        float spacing = Math.Max(240f, length / 3f);
        int count = Math.Max(1, (int)(length / spacing));
        Color bright = Lighten(s.EffectColor);
        Color clear = Color.FromArgb(0, bright.R, bright.G, bright.B);

        for (int k = 0; k < count; k++)
        {
            float head = start + (s.Time * 180f + k * spacing) % length;
            float tail = Math.Max(start, head - 56f);

            using CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(ds, clear, bright)
            {
                StartPoint = new Vector2(tail, y),
                EndPoint = new Vector2(head, y)
            };

            ds.DrawLine(new Vector2(tail, y), new Vector2(head, y), brush, thickness * 2f + 1f);
            ds.FillCircle(head, y, thickness * 1.4f + 1f, bright);
        }
    }

    private static Color GetEffectColor(RoadmapPanel panel, Color fallback)
    {
        Color? first = null;

        foreach (UIElement child in panel.Children)
        {
            if (child is RoadmapItem item)
            {
                Color c = ToColor(item.Accent, fallback);

                if (item.Status == RoadmapItemStatus.Active)
                {
                    return c;
                }

                first ??= c;
            }
        }

        return first ?? fallback;
    }

    private static float GetFlickerAlpha(float time)
    {
        double burst = Math.Sin(time * 37.0) * Math.Sin(time * 5.3);
        float alpha = burst > 0.9 ? 0.55f : 1f;
        alpha -= (float)(0.03 * (Math.Sin(time * 91.0) * 0.5 + 0.5));
        return Math.Clamp(alpha, 0.5f, 1f);
    }

    private static Color Lighten(Color c)
    {
        return Color.FromArgb(c.A, (byte)(c.R + (255 - c.R) * 0.55), (byte)(c.G + (255 - c.G) * 0.55), (byte)(c.B + (255 - c.B) * 0.55));
    }

    private static void DrawScene(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        DrawAxis(ds, s);

        if (s.Flow)
        {
            DrawAxisFlow(ds, s);
        }

        foreach (UIElement child in s.Panel.Children)
        {
            if (child is RoadmapItem item)
            {
                DrawStem(ds, s, item);
            }
        }

        foreach (UIElement child in s.Panel.Children)
        {
            if (child is RoadmapItem item)
            {
                Color accent = ItemAccent(item, s);
                DrawMarker(ds, s, item, accent);
                DrawLabel(ds, s, item, accent);

                if (s.Mode == RoadmapMode.Neon)
                {
                    DrawBrackets(ds, item, accent);
                }
            }
        }
    }

    #region Axis

    private static float GetProgressBoundary(RoadmapPanel panel)
    {
        float active = float.NaN;
        float planned = float.NaN;

        foreach (UIElement child in panel.Children)
        {
            if (child is not RoadmapItem item)
            {
                continue;
            }

            float x = (float)item.LayoutCenterX;

            if (item.Status == RoadmapItemStatus.Active)
            {
                active = float.IsNaN(active) ? x : Math.Min(active, x);
            }
            else if (item.Status == RoadmapItemStatus.Planned)
            {
                planned = float.IsNaN(planned) ? x : Math.Min(planned, x);
            }
        }

        return float.IsNaN(active) ? planned : active;
    }

    private static void DrawAxis(CanvasDrawingSession ds, RoadmapRenderState s)
    {
        RoadmapPanel panel = s.Panel;
        RoadmapMetrics m = s.Metrics;
        Color color = s.AxisColor;

        float y = (float)panel.AxisY;
        float x0 = (float)panel.AxisStartX;
        float x1 = (float)panel.AxisEndX;
        float t = (float)m.AxisThickness;
        float lineStart = x0;
        float lineEnd = x1;

        float arrowLength = 12f + t * 1.5f;
        float arrowHalf = 7f + t;

        if (m.EndCap == RoadmapAxisCap.Arrow)
        {
            lineEnd = x1 - arrowLength + 1f;
            using CanvasGeometry arrow = CanvasGeometry.CreatePolygon(ds, new[]
            {
                new Vector2(x1, y),
                new Vector2(x1 - arrowLength, y - arrowHalf),
                new Vector2(x1 - arrowLength, y + arrowHalf)
            });
            ds.FillGeometry(arrow, color);
        }
        else if (m.EndCap == RoadmapAxisCap.Dot)
        {
            ds.FillCircle(x1, y, CapRadius, color);
        }

        if (m.StartCap == RoadmapAxisCap.Arrow)
        {
            lineStart = x0 + arrowLength - 1f;
            using CanvasGeometry arrow = CanvasGeometry.CreatePolygon(ds, new[]
            {
                new Vector2(x0, y),
                new Vector2(x0 + arrowLength, y - arrowHalf),
                new Vector2(x0 + arrowLength, y + arrowHalf)
            });
            ds.FillGeometry(arrow, color);
        }
        else if (m.StartCap == RoadmapAxisCap.Dot)
        {
            ds.FillCircle(x0, y, CapRadius, color);
        }

        float split = float.IsNaN(s.Boundary) ? lineEnd : Math.Clamp(s.Boundary, lineStart, lineEnd);

        ds.DrawLine(lineStart, y, split, y, color, t);

        if (split < lineEnd)
        {
            ds.DrawLine(split, y, lineEnd, y, Dim(color), t, s.Dashed);
        }
    }

    #endregion

    #region Stems

    private static void DrawStem(CanvasDrawingSession ds, RoadmapRenderState s, RoadmapItem item)
    {
        RoadmapPanel panel = s.Panel;
        RoadmapMetrics m = s.Metrics;
        Color accent = ItemAccent(item, s);
        CanvasStrokeStyle? style = item.Status == RoadmapItemStatus.Planned ? s.Dashed : null;

        float cx = (float)item.LayoutCenterX;
        float axisY = (float)panel.AxisY;
        float side = item.LayoutSide;
        float r = (float)(m.MarkerSize / 2);
        float t = (float)m.StemThickness;
        float stemLength = (float)m.StemLength;

        switch (s.Mode)
        {
            case RoadmapMode.Simple:
                {
                    float endY = axisY + side * stemLength;
                    DrawVertical(ds, cx, axisY + side * r, endY, side, s.StemColor, t, style);
                    ds.FillCircle(cx, endY, (float)EndDotRadius, accent);
                    ds.FillCircle(cx, endY, (float)EndDotRadius * 0.38f, s.FillColor);
                    break;
                }

            case RoadmapMode.Boxed:
            case RoadmapMode.Neon:
                {
                    Color stem = s.Mode == RoadmapMode.Neon ? accent : s.StemColor;
                    DrawVertical(ds, cx, axisY + side * r, axisY + side * stemLength, side, stem, t, style);
                    break;
                }

            default:
                {
                    float inset = (float)StemInset;
                    float near = side < 0 ? (float)item.LayoutBounds.Bottom : (float)item.LayoutBounds.Top;
                    float far = side < 0 ? (float)item.LayoutBounds.Top : (float)item.LayoutBounds.Bottom;

                    DrawVertical(ds, cx, axisY + side * r, near - side * inset, side, accent, t, style);

                    if (!string.IsNullOrEmpty(item.Label))
                    {
                        float zone = (float)(side < 0 ? panel.ZoneAbove : panel.ZoneBelow);
                        float badgeNear = axisY + side * (r + stemLength + zone + (float)m.SecondStemLength);
                        DrawVertical(ds, cx, far + side * inset, badgeNear, side, accent, t, style);
                    }
                    break;
                }
        }
    }

    private static void DrawVertical(CanvasDrawingSession ds, float x, float from, float to, float direction, Color color, float thickness, CanvasStrokeStyle? style)
    {
        if ((to - from) * direction <= 1f)
        {
            return;
        }

        if (style is null)
        {
            ds.DrawLine(x, from, x, to, color, thickness);
        }
        else
        {
            ds.DrawLine(x, from, x, to, color, thickness, style);
        }
    }

    #endregion

    #region Markers

    private static void DrawMarker(CanvasDrawingSession ds, RoadmapRenderState s, RoadmapItem item, Color accent)
    {
        RoadmapMetrics m = s.Metrics;
        float cx = (float)item.LayoutCenterX;
        float cy = (float)s.Panel.AxisY;
        float r = (float)(m.MarkerSize / 2);
        bool planned = item.Status == RoadmapItemStatus.Planned;

        switch (s.Mode)
        {
            case RoadmapMode.Simple:
                if (planned)
                {
                    ds.FillCircle(cx, cy, r, s.FillColor);
                    ds.DrawCircle(cx, cy, r - 1.5f, accent, 3f);
                }
                else
                {
                    ds.FillCircle(cx, cy, r, accent);
                }
                break;

            case RoadmapMode.Boxed:
                if (planned)
                {
                    ds.FillCircle(cx, cy, r, s.FillColor);
                    ds.DrawCircle(cx, cy, r - 1.5f, accent, 3f);
                }
                else
                {
                    ds.FillCircle(cx, cy, r, accent);
                    ds.DrawCircle(cx, cy, r - 3.5f, Color.FromArgb(170, s.FillColor.R, s.FillColor.G, s.FillColor.B), 1.5f);
                }
                break;

            case RoadmapMode.Neon:
                ds.FillCircle(cx, cy, r, s.FillColor);
                ds.DrawCircle(cx, cy, r - 1.25f, accent, 2.5f);
                if (!planned)
                {
                    ds.FillCircle(cx, cy, r * 0.4f, accent);
                }
                break;

            default:
                ds.FillCircle(cx, cy, r, s.FillColor);
                ds.DrawCircle(cx, cy, r - 1.5f, accent, 3f);

                if (!string.IsNullOrEmpty(item.Glyph))
                {
                    using CanvasTextLayout layout = new CanvasTextLayout(ds, item.Glyph, s.GlyphFormat, r * 2f, r * 2f);
                    ds.DrawTextLayout(layout, cx - r, cy - r, accent);
                }
                break;
        }

        if (item.Status == RoadmapItemStatus.Active && s.Pulse >= 0f)
        {
            float growth = Math.Min(r * 1.5f, 18f);
            float radius = r + 2f + s.Pulse * growth;
            byte alpha = (byte)(200f * (1f - s.Pulse));
            ds.DrawCircle(cx, cy, radius, Color.FromArgb(alpha, accent.R, accent.G, accent.B), 2f);
        }
    }

    private static void DrawBrackets(CanvasDrawingSession ds, RoadmapItem item, Color color)
    {
        Rect b = Inflate(item.LayoutBounds, 3);
        float left = (float)b.Left;
        float top = (float)b.Top;
        float right = (float)b.Right;
        float bottom = (float)b.Bottom;
        float len = 10f;
        float t = 1.5f;

        ds.DrawLine(left, top + len, left, top, color, t);
        ds.DrawLine(left, top, left + len, top, color, t);

        ds.DrawLine(right - len, top, right, top, color, t);
        ds.DrawLine(right, top, right, top + len, color, t);

        ds.DrawLine(left, bottom - len, left, bottom, color, t);
        ds.DrawLine(left, bottom, left + len, bottom, color, t);

        ds.DrawLine(right - len, bottom, right, bottom, color, t);
        ds.DrawLine(right, bottom, right, bottom - len, color, t);
    }

    #endregion

    #region Labels

    private static void DrawLabel(CanvasDrawingSession ds, RoadmapRenderState s, RoadmapItem item, Color accent)
    {
        if (string.IsNullOrEmpty(item.Label))
        {
            return;
        }

        RoadmapMetrics m = s.Metrics;
        float cx = (float)item.LayoutCenterX;
        float axisY = (float)s.Panel.AxisY;
        float side = item.LayoutSide;
        float itemWidth = (float)m.ItemWidth;
        float labelHeight = (float)m.LabelHeight;

        using CanvasTextLayout layout = new CanvasTextLayout(ds, item.Label, s.LabelFormat, itemWidth, labelHeight);

        if (s.Mode == RoadmapMode.Icon)
        {
            float r = (float)(m.MarkerSize / 2);
            float zone = (float)(side < 0 ? s.Panel.ZoneAbove : s.Panel.ZoneBelow);
            float badgeHeight = (float)m.BadgeHeight;
            float nearY = axisY + side * (r + (float)m.StemLength + zone + (float)m.SecondStemLength);
            float centerY = nearY + side * badgeHeight / 2f;
            float badgeWidth = Math.Min(itemWidth, (float)layout.LayoutBounds.Width + 28f);
            float left = cx - badgeWidth / 2f;
            float top = centerY - badgeHeight / 2f;

            ds.FillRoundedRectangle(left, top, badgeWidth, badgeHeight, 4f, 4f, s.FillColor);
            ds.DrawRoundedRectangle(left + 0.75f, top + 0.75f, badgeWidth - 1.5f, badgeHeight - 1.5f, 4f, 4f, accent, 1.5f);
            ds.DrawTextLayout(layout, cx - itemWidth / 2f, centerY - labelHeight / 2f, accent);
        }
        else
        {
            float centerY = axisY - side * (float)m.LabelOffset;
            Color color = s.Mode == RoadmapMode.Boxed ? s.TextColor : accent;
            ds.DrawTextLayout(layout, cx - itemWidth / 2f, centerY - labelHeight / 2f, color);
        }
    }

    #endregion

    #region Helpers

    private static Color ItemAccent(RoadmapItem item, RoadmapRenderState s)
    {
        Color color = ToColor(item.Accent, s.AxisColor);
        return item.Status == RoadmapItemStatus.Planned ? Dim(color) : color;
    }

    private static Color Dim(Color color)
    {
        return Color.FromArgb((byte)(color.A * 0.5), color.R, color.G, color.B);
    }

    private static Rect Inflate(Rect rect, double amount)
    {
        return new Rect(rect.X - amount, rect.Y - amount, rect.Width + amount * 2, rect.Height + amount * 2);
    }

    private static CanvasTextFormat CreateTextFormat(string family, float size, Windows.UI.Text.FontWeight weight)
    {
        return new CanvasTextFormat
        {
            FontFamily = family,
            FontSize = size,
            FontWeight = weight,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            WordWrapping = CanvasWordWrapping.NoWrap,
            TrimmingGranularity = CanvasTextTrimmingGranularity.Character,
            TrimmingSign = CanvasTrimmingSign.Ellipsis
        };
    }

    private static string ResolveFontFamily(Microsoft.UI.Xaml.Media.FontFamily? family, string fallback)
    {
        string source = family?.Source ?? string.Empty;

        int comma = source.IndexOf(',');
        if (comma >= 0)
        {
            source = source.Substring(0, comma);
        }

        source = source.Trim();

        if (source.Length == 0 || string.Equals(source, "XamlAutoFontFamily", StringComparison.OrdinalIgnoreCase))
        {
            return fallback;
        }

        return source;
    }

    private static Color ToColor(Brush? brush, Color fallback)
    {
        switch (brush)
        {
            case SolidColorBrush solid:
                return solid.Color;

            case LinearGradientBrush gradient when gradient.GradientStops.Count > 0:
                return gradient.GradientStops[0].Color;

            case AcrylicBrush acrylic:
                return acrylic.FallbackColor;

            default:
                return fallback;
        }
    }

    #endregion
}
