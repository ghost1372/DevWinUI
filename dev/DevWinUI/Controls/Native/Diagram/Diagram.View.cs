using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;

public sealed partial class Diagram
{
    public double Zoom => _zoom;

    public void ResetView()
    {
        _zoom = 1;
        _panX = _panY = 0;
        ApplyView();
        BuildBackground();
    }

    public void ZoomBy(double factor) =>
        ZoomAt(new Point(ActualWidth / 2, ActualHeight / 2), factor);

    public void FitToContent(double margin = 48)
    {
        EnsureBounds();
        if (!_hasBounds || ActualWidth <= 0 || ActualHeight <= 0) return;

        double bw = Math.Max(_bMaxX - _bMinX, 1), bh = Math.Max(_bMaxY - _bMinY, 1);
        _zoom = Math.Clamp(Math.Min((ActualWidth - 2 * margin) / bw, (ActualHeight - 2 * margin) / bh), MinZoom, 1);
        _panX = (ActualWidth - bw * _zoom) / 2 - _bMinX * _zoom;
        _panY = (ActualHeight - bh * _zoom) / 2 - _bMinY * _zoom;
        ApplyView();
        BuildBackground();
    }

    public void CenterOn(DiagramNode node)
    {
        if (!_views.TryGetValue(node, out var v)) return;

        _panX = ActualWidth / 2 - (node.Left + v.Width / 2) * _zoom;
        _panY = ActualHeight / 2 - (node.Top + NodeHeight(v) / 2) * _zoom;
        ApplyView();
    }

    public void FocusNode(DiagramNode node)
    {
        CenterOn(node);
        if (!_views.TryGetValue(node, out var v)) return;

        DispatcherQueue.TryEnqueue(() => v.Card?.Focus(FocusState.Keyboard));
    }

    private void ApplyView()
    {
        _xf.ScaleX = _xf.ScaleY = _zoom;
        _xf.TranslateX = _panX;
        _xf.TranslateY = _panY;
        UpdateBackgroundOffset();
        UpdateZoomText();
        Cull();
        UpdateMinimap();
    }

    private void ZoomAt(Point center, double factor)
    {
        double newZoom = Math.Clamp(_zoom * factor, MinZoom, MaxZoom);
        double f = newZoom / _zoom;
        _panX = center.X - (center.X - _panX) * f;
        _panY = center.Y - (center.Y - _panY) * f;
        _zoom = newZoom;
        ApplyView();
        BuildBackground();
    }

    private void EnsureVisible(DiagramNodeView v)
    {
        const double margin = 24;
        var n = v.Node;
        double l = n.Left * _zoom + _panX, t = n.Top * _zoom + _panY;
        double r = l + v.Width * _zoom, b = t + NodeHeight(v) * _zoom;

        if (l < margin) _panX += margin - l;
        else if (r > ActualWidth - margin) _panX -= r - (ActualWidth - margin);

        if (t < margin) _panY += margin - t;
        else if (b > ActualHeight - margin) _panY -= b - (ActualHeight - margin);

        ApplyView();
    }

    private void Root_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var pt = e.GetCurrentPoint(_root);
        bool middle = pt.Properties.IsMiddleButtonPressed;
        bool emptyLeft = pt.Properties.IsLeftButtonPressed && ReferenceEquals(e.OriginalSource, _root);
        if (!middle && !emptyLeft) return;

        _panning = true;
        _panMoved = false;
        _panLeft = emptyLeft;
        _panLast = _panStart = pt.Position;
        _root!.CapturePointer(e.Pointer);
        if (emptyLeft) Focus(FocusState.Pointer);
        e.Handled = true;
    }

    private void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_panning) return;

        var p = e.GetCurrentPoint(_root).Position;
        if (Math.Abs(p.X - _panStart.X) > 4 || Math.Abs(p.Y - _panStart.Y) > 4) _panMoved = true;

        _panX += p.X - _panLast.X;
        _panY += p.Y - _panLast.Y;
        _panLast = p;
        ApplyView();
    }

    private void Root_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_panning) return;

        _panning = false;
        _root?.ReleasePointerCapture(e.Pointer);

        if (_panLeft && !_panMoved)
        {
            if (SelectionMode != DiagramSelectionMode.None) ClearSelection();
            SelectEdge(null);
        }
    }

    private void Root_PointerCaptureLost(object sender, PointerRoutedEventArgs e) => _panning = false;

    private void Root_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var pt = e.GetCurrentPoint(_root);
        int delta = pt.Properties.MouseWheelDelta;

        if (e.KeyModifiers.HasFlag(VirtualKeyModifiers.Control))
        {
            ZoomAt(pt.Position, Math.Pow(1.0015, delta));
        }
        else if (e.KeyModifiers.HasFlag(VirtualKeyModifiers.Shift))
        {
            _panX += delta;
            ApplyView();
        }
        else
        {
            _panY += delta;
            ApplyView();
        }

        e.Handled = true;
    }
}
