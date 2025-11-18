//https://github.com/benthorner/radial_controls

namespace DevWinUI;

public sealed class HaloSliderEventArgs
{
    public HaloSliderEventArgs(double angle)
    {
        Angle = angle;
    }

    public double Angle { get; private set; }
}
