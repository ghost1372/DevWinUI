using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

internal sealed partial class EyeTrackingEye
{
    public double Cx, Cy, CurX, CurY, Dilate = 1;
    public CompositeTransform Blink = new();
    public CompositeTransform Iris = new();
    public CompositeTransform Pupil = new();
    public Rectangle? Scan;
    public CompositeTransform? Ticks;
    public CompositeTransform? Arcs;
}
