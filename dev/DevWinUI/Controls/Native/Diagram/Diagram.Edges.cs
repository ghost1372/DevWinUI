using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

public sealed partial class Diagram
{
    public int LoadedNodeCount => _nodes?.Children.Count ?? 0;

    private static bool Overlaps(double x, double y, double w, double h, Rect r) =>
        x < r.X + r.Width && x + w > r.X && y < r.Y + r.Height && y + h > r.Y;

    private void Cull()
    {
        if (_nodes == null || _edges == null || ActualWidth <= 0 || ActualHeight <= 0 || _zoom <= 0) return;

        double vx = -_panX / _zoom, vy = -_panY / _zoom;
        double vw = ActualWidth / _zoom, vh = ActualHeight / _zoom;
        double m = Math.Max(0, CullMargin);

        var show = new Rect(vx - m, vy - m, vw + 2 * m, vh + 2 * m);
        var keep = new Rect(vx - 2 * m, vy - 2 * m, vw + 4 * m, vh + 4 * m);
        bool all = !EnableCulling;
        bool attached = false;

        foreach (var v in _all)
        {
            var n = v.Node;
            double h = NodeHeight(v);

            if (v.Card == null)
            {
                if (all || Overlaps(n.Left, n.Top, v.Width, h, show))
                {
                    Attach(v);
                    attached = true;
                }
            }
            else if (!all && !v.Pinned && !Overlaps(n.Left, n.Top, v.Width, h, keep))
            {
                Detach(v);
            }
        }

        foreach (var ev in _edgeViews)
        {
            if (ev.Path == null)
            {
                if (all || EdgeOverlaps(ev, show)) AttachEdge(ev);
            }
            else if (!all && !EdgeOverlaps(ev, keep))
            {
                DetachEdge(ev);
            }
        }

        if (attached) UpdateEdges();
    }

    private void Attach(DiagramNodeView v)
    {
        BuildCard(v);
        var card = v.Card!;

        card.Measure(new Size(v.Width, double.PositiveInfinity));
        double h = card.DesiredSize.Height;
        if (h > 0 && Math.Abs(h - v.LastHeight) > 0.5)
        {
            v.LastHeight = h;
            v.Node.CachedHeight = h;
            v.Node.CacheEpoch = _epoch;
            _boundsDirty = true;
            UpdateMiniRect(v);
        }

        Canvas.SetLeft(card, v.Node.Left);
        Canvas.SetTop(card, v.Node.Top);
        Canvas.SetZIndex(card, v.Z);
        _nodes!.Children.Add(card);
    }

    private void Detach(DiagramNodeView v)
    {
        if (v.Card != null) _nodes!.Children.Remove(v.Card);
        ResetCard(v);
    }

    private void AttachEdge(DiagramEdgeView ev)
    {
        ev.Path = new Path
        {
            StrokeThickness = 2,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round,
            IsHitTestVisible = false,
        };

        AutomationProperties.SetAccessibilityView(ev.Path, AccessibilityView.Raw);

        if (ev.Edge != null)
        {
            var edge = ev.Edge;
            ev.Hit = new Path { StrokeThickness = 14, Stroke = new SolidColorBrush(Microsoft.UI.Colors.Transparent) };
            AutomationProperties.SetAccessibilityView(ev.Hit, AccessibilityView.Raw);
            ev.Hit.PointerPressed += (_, e) =>
            {
                e.Handled = true;
                Focus(FocusState.Pointer);
                SelectEdge(edge);
            };
            _edges!.Children.Add(ev.Hit);
        }

        _edges!.Children.Add(ev.Path);

        if (!string.IsNullOrEmpty(ev.Label))
        {
            ev.LabelHost = new Border
            {
                Child = new TextBlock { Text = ev.Label, FontSize = 11 },
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(6, 2, 6, 2),
                IsHitTestVisible = false,
            };
            ev.LabelHost.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            ev.LabelSize = ev.LabelHost.DesiredSize;
            AutomationProperties.SetAccessibilityView(ev.LabelHost, AccessibilityView.Raw);
            ApplyLabelBrush(ev);
            _edges.Children.Add(ev.LabelHost);
        }

        UpdateEdgePath(ev);
    }

    private void DetachEdge(DiagramEdgeView ev)
    {
        if (ev.Path != null) _edges!.Children.Remove(ev.Path);
        if (ev.LabelHost != null) _edges!.Children.Remove(ev.LabelHost);
        if (ev.Hit != null) _edges!.Children.Remove(ev.Hit);
        ev.Hit = null;
        ev.DashKind = 0;
        ev.Path = null;
        ev.LabelHost = null;
    }

    private (Point A, Point B, Point C1, Point C2, Point Dir) Shape(DiagramEdgeView ev)
    {
        var f = ev.From;
        var t = ev.To;
        double fw = f.Width, fh = NodeHeight(f), tw = t.Width, th = NodeHeight(t);
        double fl = f.Node.Left, ft = f.Node.Top, tl = t.Node.Left, tt = t.Node.Top;

        if (ev.FromPin != null && ev.ToPin != null)
        {
            var pa = PinEdgeAnchor(f, ev.FromPin);
            var pb = PinEdgeAnchor(t, ev.ToPin);
            double pdist = Math.Max(48, Math.Abs(pb.X - pa.X) * 0.5);
            return (pa, pb, new Point(pa.X + pdist, pa.Y), new Point(pb.X - pdist, pb.Y), new Point(1, 0));
        }

        Point a, b, d;
        switch (EffectiveDirection)
        {
            case DiagramDirection.BottomToTop:
                a = new Point(fl + fw / 2, ft);
                b = new Point(tl + tw / 2, tt + th);
                d = new Point(0, -1);
                break;
            case DiagramDirection.LeftToRight:
                a = new Point(fl + fw, ft + fh / 2);
                b = new Point(tl, tt + th / 2);
                d = new Point(1, 0);
                break;
            case DiagramDirection.RightToLeft:
                a = new Point(fl, ft + fh / 2);
                b = new Point(tl + tw, tt + th / 2);
                d = new Point(-1, 0);
                break;
            default:
                a = new Point(fl + fw / 2, ft + fh);
                b = new Point(tl + tw / 2, tt);
                d = new Point(0, 1);
                break;
        }

        double dist = Math.Max(48, Math.Abs((b.X - a.X) * d.X + (b.Y - a.Y) * d.Y) * 0.5);
        var c1 = new Point(a.X + d.X * dist, a.Y + d.Y * dist);
        var c2 = new Point(b.X - d.X * dist, b.Y - d.Y * dist);
        return (a, b, c1, c2, d);
    }

    private bool EdgeOverlaps(DiagramEdgeView ev, Rect r)
    {
        var s = Shape(ev);
        double minX = Math.Min(Math.Min(s.A.X, s.B.X), Math.Min(s.C1.X, s.C2.X)) - 24;
        double maxX = Math.Max(Math.Max(s.A.X, s.B.X), Math.Max(s.C1.X, s.C2.X)) + 24;
        double minY = Math.Min(Math.Min(s.A.Y, s.B.Y), Math.Min(s.C1.Y, s.C2.Y)) - 24;
        double maxY = Math.Max(Math.Max(s.A.Y, s.B.Y), Math.Max(s.C1.Y, s.C2.Y)) + 24;
        return Overlaps(minX, minY, maxX - minX, maxY - minY, r);
    }

    private void UpdateEdges()
    {
        foreach (var ev in _edgeViews)
            if (ev.Path != null) UpdateEdgePath(ev);
    }

    private void UpdateEdgePath(DiagramEdgeView ev)
    {
        if (ev.Path == null) return;

        var s = Shape(ev);
        ev.Path.Data = BuildGeometry(ev, s, ShowArrows, out var mid);
        if (ev.Hit != null) ev.Hit.Data = BuildGeometry(ev, s, false, out _);

        if (ev.LabelHost != null)
        {
            Canvas.SetLeft(ev.LabelHost, mid.X - ev.LabelSize.Width / 2);
            Canvas.SetTop(ev.LabelHost, mid.Y - ev.LabelSize.Height / 2);
        }

        ApplyEdgeBrush(ev);
    }

    private PathGeometry BuildGeometry(DiagramEdgeView ev, (Point A, Point B, Point C1, Point C2, Point Dir) s, bool arrow, out Point mid)
    {
        var geometry = new PathGeometry();
        Point endDir = s.Dir;

        switch (EdgeStyle)
        {
            case DiagramEdgeStyle.Straight:
            {
                var fig = new PathFigure { StartPoint = s.A, IsClosed = false, IsFilled = false };
                fig.Segments.Add(new LineSegment { Point = s.B });
                geometry.Figures.Add(fig);
                endDir = Normalize(s.B.X - s.A.X, s.B.Y - s.A.Y, s.Dir);
                mid = new Point((s.A.X + s.B.X) / 2, (s.A.Y + s.B.Y) / 2);
                break;
            }

            case DiagramEdgeStyle.Orthogonal:
            {
                bool vertical = !(Horizontal || ev.FromPin != null);
                var pts = new List<Point>();
                if (vertical)
                {
                    double my = s.A.Y + (s.B.Y - s.A.Y) / 2;
                    pts.Add(s.A);
                    pts.Add(new Point(s.A.X, my));
                    pts.Add(new Point(s.B.X, my));
                    pts.Add(s.B);
                    mid = new Point((s.A.X + s.B.X) / 2, my);
                }
                else
                {
                    double mx = s.A.X + (s.B.X - s.A.X) / 2;
                    pts.Add(s.A);
                    pts.Add(new Point(mx, s.A.Y));
                    pts.Add(new Point(mx, s.B.Y));
                    pts.Add(s.B);
                    mid = new Point(mx, (s.A.Y + s.B.Y) / 2);
                }

                geometry.Figures.Add(Rounded(pts, Math.Max(0, EdgeCornerRadius)));
                break;
            }

            default:
            {
                var fig = new PathFigure { StartPoint = s.A, IsClosed = false, IsFilled = false };
                fig.Segments.Add(new BezierSegment { Point1 = s.C1, Point2 = s.C2, Point3 = s.B });
                geometry.Figures.Add(fig);
                mid = new Point((s.A.X + 3 * s.C1.X + 3 * s.C2.X + s.B.X) / 8, (s.A.Y + 3 * s.C1.Y + 3 * s.C2.Y + s.B.Y) / 8);
                break;
            }
        }

        if (arrow) geometry.Figures.Add(Arrow(s.B, endDir));
        return geometry;
    }

    private static Point Normalize(double dx, double dy, Point fallback)
    {
        double len = Math.Sqrt(dx * dx + dy * dy);
        return len < 0.001 ? fallback : new Point(dx / len, dy / len);
    }

    private static PathFigure Arrow(Point tip, Point dir)
    {
        const double length = 10, half = 5;
        double bx = tip.X - dir.X * length, by = tip.Y - dir.Y * length;

        var fig = new PathFigure { StartPoint = tip, IsClosed = true, IsFilled = true };
        fig.Segments.Add(new LineSegment { Point = new Point(bx - dir.Y * half, by + dir.X * half) });
        fig.Segments.Add(new LineSegment { Point = new Point(bx + dir.Y * half, by - dir.X * half) });
        return fig;
    }

    private static PathFigure Rounded(List<Point> source, double radius)
    {
        var p = new List<Point>();
        foreach (var pt in source)
        {
            if (p.Count == 0 || Math.Abs(pt.X - p[^1].X) + Math.Abs(pt.Y - p[^1].Y) > 0.01) p.Add(pt);
        }

        var fig = new PathFigure { StartPoint = p[0], IsClosed = false, IsFilled = false };

        for (int i = 1; i < p.Count - 1; i++)
        {
            Point prev = p[i - 1], cur = p[i], next = p[i + 1];
            double l1 = Math.Sqrt((prev.X - cur.X) * (prev.X - cur.X) + (prev.Y - cur.Y) * (prev.Y - cur.Y));
            double l2 = Math.Sqrt((next.X - cur.X) * (next.X - cur.X) + (next.Y - cur.Y) * (next.Y - cur.Y));
            double r = Math.Min(radius, Math.Min(l1, l2) / 2);
            double cross = (cur.X - prev.X) * (next.Y - cur.Y) - (cur.Y - prev.Y) * (next.X - cur.X);

            if (r < 0.5 || Math.Abs(cross) < 1e-6)
            {
                fig.Segments.Add(new LineSegment { Point = cur });
                continue;
            }

            fig.Segments.Add(new LineSegment { Point = new Point(cur.X + (prev.X - cur.X) / l1 * r, cur.Y + (prev.Y - cur.Y) / l1 * r) });
            fig.Segments.Add(new ArcSegment
            {
                Point = new Point(cur.X + (next.X - cur.X) / l2 * r, cur.Y + (next.Y - cur.Y) / l2 * r),
                Size = new Size(r, r),
                SweepDirection = cross > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
            });
        }

        fig.Segments.Add(new LineSegment { Point = p[^1] });
        return fig;
    }

    private void ApplyEdgeBrushes()
    {
        foreach (var ev in _edgeViews)
            if (ev.Path != null) ApplyEdgeBrush(ev);
    }

    private void ApplyEdgeBrush(DiagramEdgeView ev)
    {
        if (ev.Path == null) return;

        bool selected = ev.Edge != null && ReferenceEquals(ev.Edge, _selectedEdge);
        bool lit = ev.From.Hot || ev.To.Hot || ev.From.Selected || ev.To.Selected;
        Brush? brush;
        if (ev.Conflict != null) brush = ConflictFill;
        else if (selected) brush = SelectedNodeBorderBrush ?? ConnectorBrush;
        else if (ev.FromPin != null && ColorEdgesByType) brush = TypeBrush(ev.TypeName ?? string.Empty);
        else brush = lit ? ev.From.HueBrush : ConnectorBrush;

        ev.Path.Stroke = brush;
        ev.Path.Fill = ShowArrows ? brush : null;
        ev.Path.StrokeThickness = selected ? 3.5 : lit && ev.FromPin != null ? 2.5 : 2;
        SetDash(ev);
    }

    private void SetDash(DiagramEdgeView ev)
    {
        if (ev.Path == null) return;

        int kind = ev.Conflict != null || ev.Dashed ? 1 : (ev.Animated || AnimateEdges) ? 2 : 0;
        if (kind == ev.DashKind) return;

        ev.DashKind = kind;
        ev.Path.StrokeDashArray = kind == 1 ? new DoubleCollection { 5, 4 } : kind == 2 ? new DoubleCollection { 6, 6 } : null;
        if (kind != 2) ev.Path.StrokeDashOffset = 0;
    }

    private void ApplyLabelBrushes()
    {
        foreach (var ev in _edgeViews) ApplyLabelBrush(ev);
    }

    private void ApplyLabelBrush(DiagramEdgeView ev)
    {
        if (ev.LabelHost == null) return;

        ev.LabelHost.Background = NodeBackground;
        ev.LabelHost.BorderBrush = NodeBorderBrush;
        if (ev.LabelHost.Child is TextBlock t) t.Foreground = NodeCaptionForeground;
    }
}
