using Microsoft.UI.Xaml.Input;
using Windows.System;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

public sealed partial class Diagram
{
    private DiagramLinkDrag? _link;
    private DiagramNodeView? _hoverView;
    private DiagramPin? _hoverPin;
    private DiagramPinVisual? _hoverPv;
    private DiagramEdge? _selectedEdge;
    private readonly SolidColorBrush _fallbackConflict = new(Windows.UI.Color.FromArgb(255, 0xEF, 0x44, 0x44));

    public event EventHandler<DiagramConnectEventArgs>? ConnectionRequested;

    public event EventHandler<DiagramEdgeEventArgs>? EdgeAdded;

    public event EventHandler<DiagramEdgeEventArgs>? EdgeRemoved;

    public event EventHandler<DiagramEdgeEventArgs>? EdgeSelect;

    public event EventHandler<DiagramNodeEventArgs>? NodeRemoved;

    public event EventHandler? ConflictsChanged;

    public DiagramEdge? SelectedEdge => _selectedEdge;

    private Brush ConflictFill => ConflictBrush ?? _fallbackConflict;

    public void SelectEdge(DiagramEdge? edge)
    {
        if (ReferenceEquals(_selectedEdge, edge)) return;

        _selectedEdge = edge;
        if (edge != null) ClearSelection();
        ApplyEdgeBrushes();
        if (edge != null) EdgeSelect?.Invoke(this, new DiagramEdgeEventArgs(edge));
    }

    public void DeleteSelected()
    {
        if (!AllowDelete) return;

        if (_selectedEdge is { } edge)
        {
            _selectedEdge = null;
            if (Edges.Remove(edge)) EdgeRemoved?.Invoke(this, new DiagramEdgeEventArgs(edge));
        }

        foreach (var node in SelectedNodes.ToArray()) RemoveNode(node);
    }

    public void RemoveNode(DiagramNode node)
    {
        SelectedNodes.Remove(node);

        if (node.Id != null)
        {
            foreach (var e in Edges.Where(e => e.From == node.Id || e.To == node.Id).ToList())
            {
                Edges.Remove(e);
                EdgeRemoved?.Invoke(this, new DiagramEdgeEventArgs(e));
            }
        }

        if (!Nodes.Remove(node))
        {
            foreach (var n in EnumerateNodes().ToList())
            {
                if (n.Children.Remove(node)) break;
            }
        }

        NodeRemoved?.Invoke(this, new DiagramNodeEventArgs(node));
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled) return;

        if (e.Key == VirtualKey.Delete && AllowDelete)
        {
            e.Handled = true;
            DeleteSelected();
        }
        else if (e.Key == VirtualKey.Escape)
        {
            if (_link != null) EndLink(null, false);
            SelectEdge(null);
        }
    }

    private bool CheckConnect(DiagramNodeView a, DiagramPin ap, DiagramNodeView b, DiagramPin bp, out string? error)
    {
        error = null;
        if (a == b || ap.IsOutput == bp.IsOutput)
        {
            error = "invalid connection";
            return false;
        }

        var (sv, sp, tv, tp) = ap.IsOutput ? (a, ap, b, bp) : (b, bp, a, ap);

        if (sp.IsExec != tp.IsExec)
        {
            error = "kind mismatch: exec vs data";
            return false;
        }

        if (!sp.IsExec)
        {
            var src = DiagramTypeRef.Subst(DiagramTypeRef.Parse(sp.Type), sv.Bindings, true);
            var tmp = new Dictionary<int, DiagramTypeRef>(tv.Bindings);
            if (!DiagramTypeRef.Unify(DiagramTypeRef.Parse(tp.Type), src, tmp, out var err))
            {
                error = $"type mismatch: {err}";
                return false;
            }
        }

        return true;
    }

    private void TryConnect(DiagramNodeView a, DiagramPin ap, DiagramNodeView b, DiagramPin bp)
    {
        if (!CheckConnect(a, ap, b, bp, out _) && !AllowConflictingConnections) return;

        var (sv, sp, tv, tp) = ap.IsOutput ? (a, ap, b, bp) : (b, bp, a, ap);

        var args = new DiagramConnectEventArgs(sv.Node, sp, tv.Node, tp);
        ConnectionRequested?.Invoke(this, args);
        if (args.Cancel) return;

        sv.Node.Id ??= Guid.NewGuid().ToString();
        tv.Node.Id ??= Guid.NewGuid().ToString();
        string from = sv.Node.Id, to = tv.Node.Id;

        if (Edges.Any(e => e.From == from && e.To == to && e.FromPin == sp.Key && e.ToPin == tp.Key)) return;

        var stale = Edges.Where(e =>
            (!tp.IsExec && e.To == to && e.ToPin == tp.Key) ||
            (sp.IsExec && e.From == from && e.FromPin == sp.Key)).ToList();

        foreach (var old in stale)
        {
            Edges.Remove(old);
            EdgeRemoved?.Invoke(this, new DiagramEdgeEventArgs(old));
        }

        var edge = new DiagramEdge { From = from, To = to, FromPin = sp.Key, ToPin = tp.Key };
        Edges.Add(edge);
        EdgeAdded?.Invoke(this, new DiagramEdgeEventArgs(edge));
    }

    private void Pin_PointerPressed(DiagramNodeView v, DiagramPin pin, Border handle, PointerRoutedEventArgs e)
    {
        if (!AllowConnect || _edges == null || !e.GetCurrentPoint(handle).Properties.IsLeftButtonPressed) return;

        e.Handled = true;

        var line = new Path
        {
            StrokeThickness = 2,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            IsHitTestVisible = false,
            Stroke = PinBrush(v, pin),
        };
        _edges.Children.Add(line);

        _link = new DiagramLinkDrag { View = v, Pin = pin, Line = line, Handle = handle };
        handle.CapturePointer(e.Pointer);
        DimIncompatible(_link);
        UpdateLink(e);
    }

    private void Pin_PointerMoved(PointerRoutedEventArgs e)
    {
        if (_link == null) return;

        UpdateLink(e);
        e.Handled = true;
    }

    private void Pin_PointerReleased(PointerRoutedEventArgs e)
    {
        if (_link == null) return;

        UpdateLink(e);
        EndLink(e.Pointer, true);
        e.Handled = true;
    }

    private void EndLink(Pointer? pointer, bool commit)
    {
        var l = _link;
        if (l == null) return;

        _link = null;
        if (pointer != null) l.Handle.ReleasePointerCapture(pointer);
        _edges?.Children.Remove(l.Line);
        DimIncompatible(null);

        var hv = _hoverView;
        var hp = _hoverPin;
        SetHover(null, null);

        if (commit && hv != null && hp != null) TryConnect(l.View, l.Pin, hv, hp);
    }

    private void UpdateLink(PointerRoutedEventArgs e)
    {
        var l = _link;
        if (l == null || _nodes == null) return;

        var p = e.GetCurrentPoint(_nodes).Position;
        FindPinAt(p, l, out var hv, out var hp);
        SetHover(hv, hp);

        bool ok = true;
        var end = p;
        if (hv != null && hp != null)
        {
            ok = CheckConnect(l.View, l.Pin, hv, hp, out _);
            end = PinEdgeAnchor(hv, hp);
        }

        var a = PinEdgeAnchor(l.View, l.Pin);
        double dist = Math.Max(48, Math.Abs(end.X - a.X) * 0.5);
        double s = l.Pin.IsOutput ? 1 : -1;

        var fig = new PathFigure { StartPoint = a, IsClosed = false, IsFilled = false };
        fig.Segments.Add(new BezierSegment
        {
            Point1 = new Point(a.X + s * dist, a.Y),
            Point2 = new Point(end.X - s * dist, end.Y),
            Point3 = end,
        });

        var g = new PathGeometry();
        g.Figures.Add(fig);
        l.Line.Data = g;
        l.Line.Stroke = ok ? PinBrush(l.View, l.Pin) : ConflictFill;
    }

    private void FindPinAt(Point p, DiagramLinkDrag l, out DiagramNodeView? view, out DiagramPin? pin)
    {
        view = null;
        pin = null;
        double best = 20 * 20;

        foreach (var v in _all)
        {
            if (v == l.View || v.Card == null) continue;

            foreach (var cand in l.Pin.IsOutput ? v.Node.Inputs : v.Node.Outputs)
            {
                var c = PinCenter(v, cand);
                double d = (c.X - p.X) * (c.X - p.X) + (c.Y - p.Y) * (c.Y - p.Y);
                if (d < best)
                {
                    best = d;
                    view = v;
                    pin = cand;
                }
            }
        }
    }

    private void SetHover(DiagramNodeView? v, DiagramPin? p)
    {
        if (_hoverPv != null) _hoverPv.Scale.ScaleX = _hoverPv.Scale.ScaleY = 1;
        _hoverPv = null;
        _hoverView = v;
        _hoverPin = p;

        if (v?.Pins != null && p != null && v.Pins.TryGetValue(p, out var pv))
        {
            pv.Scale.ScaleX = pv.Scale.ScaleY = 1.3;
            _hoverPv = pv;
        }
    }

    private void DimIncompatible(DiagramLinkDrag? l)
    {
        foreach (var v in _all)
        {
            if (v.Pins == null) continue;

            foreach (var pv in v.Pins.Values)
            {
                bool ok = l == null || v == l.View || pv.Pin.IsOutput == l.Pin.IsOutput ||
                          CheckConnect(l.View, l.Pin, v, pv.Pin, out _);
                pv.Handle.Opacity = ok ? 1 : 0.3;
            }
        }
    }
}
