using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

internal sealed partial class FizzyCoverParticle
{
    public Shape El = null!;
    public SolidColorBrush Brush = null!;
    public CompositeTransform T = null!;
    public double X, Y;
    public double Speed;
    public double Phase, Freq;
    public double Rot, SpinRate;
    public double Size;
    public uint Argb;
}
