using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;

namespace DevWinUI;

public partial class PureColorBackgroundRenderer : RendererBase
{
    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        double finalOpacity = pureColorOverlayOpacity / 100.0;

        if (!isEnabled || finalOpacity <= 0) return;

        var ds = args.DrawingSession;
        var bounds = new Rect(0, 0, sender.Size.Width, sender.Size.Height);

        ds.FillRectangle(
            bounds,
            overlayColor.WithAlpha((byte)(finalOpacity * 255))
        );
    }
}
