namespace DevWinUI;
public record DragMoveAndResizeInfo(DragMoveAndResizeMode Mode)
{
    /// <summary>
    /// Default: 10
    /// </summary>
    public int MinOffset { get; set; } = 10;

    /// <summary>
    /// Default: 10
    /// </summary>
    public int DraggableBorderThickness { get; set; } = 10;

    /// <summary>
    /// Default: (1280, 720)
    /// </summary>
    public (int X, int Y) MinSize { get; set; } = (1280, 720);

    /// <summary>
    /// Default: <see cref="WindowHelper.GetScreenSize"/>
    /// </summary>
    public (int X, int Y) MaxSize { get; set; } = WindowHelper.GetScreenSize();
}
