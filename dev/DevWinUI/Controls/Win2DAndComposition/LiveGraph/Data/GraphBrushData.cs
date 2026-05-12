namespace DevWinUI;

public partial class GraphBrushData
{
    public ICanvasBrush Brush { get; set; }
    public ICanvasBrush OpacityBrush { get; set; }
    public ICanvasBrush BorderBrush { get; set; }
    public float StrokeWidth { get; set; }
    public CanvasStrokeStyle StrokeStyle { get; set; }

    internal bool IsDisposed { get; set; }

    public GraphBrushData(ICanvasBrush brush, ICanvasBrush opacityBrush, ICanvasBrush borderBrush, float strokeWidth, CanvasStrokeStyle strokeStyle)
    {
        Brush = brush;
        OpacityBrush = opacityBrush;
        BorderBrush = borderBrush;
        StrokeWidth = strokeWidth;
        StrokeStyle = strokeStyle;
    }
    public GraphBrushData(ICanvasBrush brush, ICanvasBrush opacityBrush, ICanvasBrush borderBrush, float strokeWidth)
    {
        Brush = brush;
        OpacityBrush = opacityBrush;
        BorderBrush = borderBrush;
        StrokeWidth = strokeWidth;
        StrokeStyle = null;
    }
    public GraphBrushData(ICanvasBrush brush, ICanvasBrush opacityBrush, ICanvasBrush borderBrush)
    {
        Brush = brush;
        OpacityBrush = opacityBrush;
        BorderBrush = borderBrush;
        StrokeWidth = 2f;
        StrokeStyle = null;
    }
    public GraphBrushData(ICanvasBrush brush, ICanvasBrush opacityBrush)
    {
        Brush = brush;
        OpacityBrush = opacityBrush;
        BorderBrush = null;
        StrokeWidth = 2f;
        StrokeStyle = null;
    }
    public GraphBrushData(ICanvasBrush brush)
    {
        Brush = brush;
        OpacityBrush = null;
        BorderBrush = null;
        StrokeWidth = 2f;
        StrokeStyle = null;
    }

    public GraphBrushData()
    {
        
    }
}
