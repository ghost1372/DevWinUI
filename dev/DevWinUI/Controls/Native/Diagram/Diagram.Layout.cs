namespace DevWinUI;

public sealed partial class Diagram
{
    private double _contentW, _contentH;

    private DiagramDirection EffectiveDirection =>
        Layout == DiagramLayout.Layered ? Direction : DiagramDirection.TopToBottom;

    private bool Horizontal => EffectiveDirection is DiagramDirection.LeftToRight or DiagramDirection.RightToLeft;

    public void ArrangeAll()
    {
        foreach (var n in EnumerateNodes())
        {
            n.OffsetX = 0;
            n.OffsetY = 0;
        }

        _layoutDirty = true;
        LayoutNodes();
    }

    private void LayoutNodes()
    {
        if (_nodes == null) return;

        if (Layout == DiagramLayout.Layered)
        {
            ArrangeLayered();
        }
        else if (!ArrangeManual())
        {
            return;
        }

        foreach (var v in _all)
        {
            var n = v.Node;
            n.Left = v.BaseLeft + n.OffsetX;
            n.Top = v.BaseTop + n.OffsetY;

            if (v.Card != null)
            {
                Canvas.SetLeft(v.Card, n.Left);
                Canvas.SetTop(v.Card, n.Top);
            }
        }

        _boundsDirty = true;
        UpdateMiniRects();
        Cull();
        UpdateEdges();
        UpdateMinimap();
        TryAutoFit();
    }

    private void TryAutoFit()
    {
        if (!_fitPending || !AutoFit || _all.Count == 0 || ActualWidth <= 0 || ActualHeight <= 0) return;

        _fitPending = false;
        FitToContent();
    }

    private bool ArrangeManual()
    {
        if (Layout == DiagramLayout.Free)
        {
            foreach (var v in _all)
            {
                v.BaseLeft = v.Node.CanvasX;
                v.BaseTop = v.Node.CanvasY;
            }

            return true;
        }

        double w = WorldWidth > 0 ? WorldWidth : ActualWidth;
        if (w <= 0) return false;

        foreach (var v in _all)
        {
            v.BaseLeft = Math.Clamp(v.Node.X, 0, 1) * w - v.Width / 2;
            v.BaseTop = Pad + v.Node.Row * RowSpacing;
        }

        return true;
    }

    private void ArrangeLayered()
    {
        if (_all.Count == 0) return;

        if (_layoutDirty)
        {
            ComputeLayered();
            _layoutDirty = false;
        }

        double shiftX = !Horizontal ? Math.Max(0, (ActualWidth - _contentW) / 2) : 0;
        double shiftY = Horizontal ? Math.Max(0, (ActualHeight - _contentH) / 2) : 0;

        foreach (var v in _all)
        {
            v.BaseLeft = v.RawLeft + shiftX;
            v.BaseTop = v.RawTop + shiftY;
        }
    }

    private double CrossSize(DiagramNodeView v) => Horizontal ? NodeHeight(v) : v.Width;

    private double FlowSize(DiagramNodeView v) => Horizontal ? v.Width : NodeHeight(v);

    private void ComputeLayered()
    {
        foreach (var v in _all)
        {
            MeasureNode(v);
            v.LIn.Clear();
            v.LOut.Clear();
        }

        foreach (var ev in _edgeViews)
        {
            ev.From.LOut.Add(ev.To);
            ev.To.LIn.Add(ev.From);
        }

        var queue = new Queue<DiagramNodeView>();
        var done = new HashSet<DiagramNodeView>();
        foreach (var v in _all)
        {
            v.Layer = 0;
            v.Pending = v.LIn.Count;
            if (v.Pending == 0) queue.Enqueue(v);
        }

        while (done.Count < _all.Count)
        {
            if (queue.Count == 0)
            {
                var forced = _all.First(n => !done.Contains(n));
                forced.Pending = 0;
                queue.Enqueue(forced);
            }

            var u = queue.Dequeue();
            if (!done.Add(u)) continue;

            foreach (var w in u.LOut)
            {
                if (done.Contains(w)) continue;

                w.Layer = Math.Max(w.Layer, u.Layer + 1);
                if (--w.Pending == 0) queue.Enqueue(w);
            }
        }

        int max = _all.Max(n => n.Layer);
        var layers = new List<List<DiagramNodeView>>();
        for (int i = 0; i <= max; i++) layers.Add(new List<DiagramNodeView>());
        foreach (var v in _all) layers[v.Layer].Add(v);
        foreach (var layer in layers)
            for (int i = 0; i < layer.Count; i++) layer[i].Order = i;

        for (int pass = 0; pass < 4; pass++)
        {
            for (int i = 1; i < layers.Count; i++) SortLayer(layers[i], n => n.LIn);
            for (int i = layers.Count - 2; i >= 0; i--) SortLayer(layers[i], n => n.LOut);
        }

        double gap = Math.Max(0, SiblingSpacing);
        foreach (var layer in layers)
        {
            double x = 0;
            foreach (var n in layer)
            {
                double s = CrossSize(n);
                n.Center = x + s / 2;
                x += s + gap;
            }
        }

        for (int pass = 0; pass < 6; pass++)
        {
            for (int i = 1; i < layers.Count; i++) Place(layers[i], n => n.LIn, gap);
            for (int i = layers.Count - 2; i >= 0; i--) Place(layers[i], n => n.LOut, gap);
        }

        double minCross = double.MaxValue, maxCross = double.MinValue;
        foreach (var v in _all)
        {
            double s = CrossSize(v);
            minCross = Math.Min(minCross, v.Center - s / 2);
            maxCross = Math.Max(maxCross, v.Center + s / 2);
        }

        var layerPos = new double[layers.Count];
        double flow = 0;
        for (int i = 0; i < layers.Count; i++)
        {
            layerPos[i] = flow;
            flow += layers[i].Max(FlowSize) + Math.Max(0, LevelSpacing);
        }

        double totalFlow = flow - Math.Max(0, LevelSpacing);
        var dir = EffectiveDirection;

        for (int i = 0; i < layers.Count; i++)
        {
            foreach (var n in layers[i])
            {
                double cross = Pad + n.Center - CrossSize(n) / 2 - minCross;
                double fs = FlowSize(n);

                switch (dir)
                {
                    case DiagramDirection.BottomToTop:
                        n.RawLeft = cross;
                        n.RawTop = Pad + totalFlow - layerPos[i] - fs;
                        break;
                    case DiagramDirection.LeftToRight:
                        n.RawLeft = Pad + layerPos[i];
                        n.RawTop = cross;
                        break;
                    case DiagramDirection.RightToLeft:
                        n.RawLeft = Pad + totalFlow - layerPos[i] - fs;
                        n.RawTop = cross;
                        break;
                    default:
                        n.RawLeft = cross;
                        n.RawTop = Pad + layerPos[i];
                        break;
                }
            }
        }

        double crossExtent = maxCross - minCross + 2 * Pad;
        double flowExtent = totalFlow + 2 * Pad;
        _contentW = Horizontal ? flowExtent : crossExtent;
        _contentH = Horizontal ? crossExtent : flowExtent;
    }

    private static void SortLayer(List<DiagramNodeView> layer, Func<DiagramNodeView, List<DiagramNodeView>> neighbors)
    {
        var keys = new Dictionary<DiagramNodeView, double>();
        foreach (var n in layer)
        {
            var nb = neighbors(n);
            keys[n] = nb.Count == 0 ? n.Order : nb.Average(x => x.Order);
        }

        var sorted = layer.OrderBy(n => keys[n]).ToList();
        layer.Clear();
        layer.AddRange(sorted);
        for (int i = 0; i < layer.Count; i++) layer[i].Order = i;
    }

    private void Place(List<DiagramNodeView> layer, Func<DiagramNodeView, List<DiagramNodeView>> neighbors, double gap)
    {
        int count = layer.Count;
        if (count == 0) return;

        var desired = new double[count];
        var size = new double[count];
        for (int i = 0; i < count; i++)
        {
            size[i] = CrossSize(layer[i]);
            var nb = neighbors(layer[i]);
            desired[i] = nb.Count == 0 ? layer[i].Center : nb.Average(x => x.Center);
        }

        var forward = new double[count];
        var backward = new double[count];

        forward[0] = desired[0];
        for (int i = 1; i < count; i++)
            forward[i] = Math.Max(desired[i], forward[i - 1] + (size[i - 1] + size[i]) / 2 + gap);

        backward[count - 1] = desired[count - 1];
        for (int i = count - 2; i >= 0; i--)
            backward[i] = Math.Min(desired[i], backward[i + 1] - (size[i] + size[i + 1]) / 2 - gap);

        for (int i = 0; i < count; i++) layer[i].Center = (forward[i] + backward[i]) / 2;
    }

    private void EnsureBounds()
    {
        if (!_boundsDirty) return;
        _boundsDirty = false;

        _hasBounds = _all.Count > 0;
        _bMinX = _bMinY = double.MaxValue;
        _bMaxX = _bMaxY = double.MinValue;

        foreach (var v in _all)
        {
            _bMinX = Math.Min(_bMinX, v.Node.Left);
            _bMinY = Math.Min(_bMinY, v.Node.Top);
            _bMaxX = Math.Max(_bMaxX, v.Node.Left + v.Width);
            _bMaxY = Math.Max(_bMaxY, v.Node.Top + NodeHeight(v));
        }
    }
}
