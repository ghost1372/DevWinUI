namespace DevWinUI;

internal sealed partial class MotionWallCard : IDisposable
{
    public MotionWallCard(CanvasRenderTarget image, CanvasRenderTarget? gray)
    {
        Image = image;
        Gray = gray;
    }

    public CanvasRenderTarget Image { get; }

    public CanvasRenderTarget? Gray { get; }

    public void Dispose()
    {
        Gray?.Dispose();
        Image.Dispose();
    }
}
