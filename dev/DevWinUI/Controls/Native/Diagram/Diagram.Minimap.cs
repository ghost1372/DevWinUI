using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public sealed partial class Diagram
{
    private void OnShowMinimapChanged()
    {
        if (_mini == null) return;

        _mini.Visibility = ShowMinimap ? Visibility.Visible : Visibility.Collapsed;
        UpdateMinimap();
    }

    private void RebuildMinimap()
    {
        if (_miniCanvas == null) return;

        _miniCanvas.Children.Clear();
        _miniRects.Clear();
        _miniCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, MiniW, MiniH) };

        _miniWorld = new Canvas { RenderTransform = _miniXf, IsHitTestVisible = false };
        _miniCanvas.Children.Add(_miniWorld);

        foreach (var v in _all)
        {
            var rect = new Rectangle
            {
                Fill = v.HueBrush,
                Opacity = v.Match ? 0.85 : 0.2,
                RadiusX = 8,
                RadiusY = 8,
                IsHitTestVisible = false,
            };
            _miniRects[v] = rect;
            _miniWorld.Children.Add(rect);
        }

        _miniView = new Rectangle
        {
            Stroke = ViewportStroke,
            StrokeThickness = 1.5,
            Fill = ViewportFill,
            RadiusX = 2,
            RadiusY = 2,
            IsHitTestVisible = false,
        };
        _miniCanvas.Children.Add(_miniView);
    }

    private void UpdateMiniRect(DiagramNodeView v)
    {
        if (!_miniRects.TryGetValue(v, out var rect)) return;

        rect.Width = v.Width;
        rect.Height = NodeHeight(v);
        rect.Opacity = v.Match ? 0.85 : 0.2;
        Canvas.SetLeft(rect, v.Node.Left);
        Canvas.SetTop(rect, v.Node.Top);
    }

    private void UpdateMiniRects()
    {
        foreach (var v in _all) UpdateMiniRect(v);
    }

    private void ApplyViewportBrushes()
    {
        if (_miniView == null) return;

        _miniView.Stroke = ViewportStroke;
        _miniView.Fill = ViewportFill;
    }

    private void UpdateMinimap()
    {
        if (_mini == null || _miniCanvas == null || _miniView == null || !ShowMinimap) return;

        double w = ActualWidth, h = ActualHeight;
        if (w <= 0 || h <= 0 || _zoom <= 0) return;

        double vx = -_panX / _zoom, vy = -_panY / _zoom, vw = w / _zoom, vh = h / _zoom;

        if (!_miniDragging)
        {
            EnsureBounds();

            double minX = vx, minY = vy, maxX = vx + vw, maxY = vy + vh;
            if (_hasBounds)
            {
                minX = Math.Min(minX, _bMinX);
                minY = Math.Min(minY, _bMinY);
                maxX = Math.Max(maxX, _bMaxX);
                maxY = Math.Max(maxY, _bMaxY);
            }

            double bw = Math.Max(maxX - minX, 1), bh = Math.Max(maxY - minY, 1);
            double aw = MiniW - 2 * MiniPad, ah = MiniH - 2 * MiniPad;
            _miniScale = Math.Min(aw / bw, ah / bh);
            _miniOx = MiniPad + (aw - bw * _miniScale) / 2 - minX * _miniScale;
            _miniOy = MiniPad + (ah - bh * _miniScale) / 2 - minY * _miniScale;
        }

        _miniXf.ScaleX = _miniXf.ScaleY = _miniScale;
        _miniXf.TranslateX = _miniOx;
        _miniXf.TranslateY = _miniOy;

        _miniView.Width = Math.Max(2, vw * _miniScale);
        _miniView.Height = Math.Max(2, vh * _miniScale);
        Canvas.SetLeft(_miniView, vx * _miniScale + _miniOx);
        Canvas.SetTop(_miniView, vy * _miniScale + _miniOy);
    }

    private void Mini_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
        if (_mini == null || !e.GetCurrentPoint(_mini).Properties.IsLeftButtonPressed) return;

        _miniDragging = true;
        _mini.CapturePointer(e.Pointer);
        MiniNavigate(e);
    }

    private void Mini_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_miniDragging) return;

        MiniNavigate(e);
        e.Handled = true;
    }

    private void Mini_PointerReleased(object sender, PointerRoutedEventArgs e) => EndMiniDrag(e.Pointer);

    private void Mini_PointerCaptureLost(object sender, PointerRoutedEventArgs e) => EndMiniDrag(null);

    private void Mini_PointerWheelChanged(object sender, PointerRoutedEventArgs e) => e.Handled = true;

    private void EndMiniDrag(Pointer? pointer)
    {
        if (!_miniDragging) return;

        _miniDragging = false;
        if (pointer != null) _mini?.ReleasePointerCapture(pointer);
        UpdateMinimap();
    }

    private void MiniNavigate(PointerRoutedEventArgs e)
    {
        if (_miniCanvas == null || _miniScale <= 0) return;

        var p = e.GetCurrentPoint(_miniCanvas).Position;
        double wx = (p.X - _miniOx) / _miniScale;
        double wy = (p.Y - _miniOy) / _miniScale;

        _panX = ActualWidth / 2 - wx * _zoom;
        _panY = ActualHeight / 2 - wy * _zoom;
        ApplyView();
    }
}
