namespace DevWinUI;

public sealed class HaloSliderEventArgs
{
    public HaloSliderEventArgs(double angle)
    {
        Angle = angle;
    }

    public double Angle { get; private set; }
}
