//https://github.com/benthorner/radial_controls

namespace DevWinUI;

internal struct HaloVector
{
    public double X, Y;

    public HaloVector(double x, double y)
    {
        X = x; Y = y;
    }

    public double Length
    {
        get
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }
    }

    public double AngleTo(HaloVector other)
    {
        var dotProduct = DotProduct(other);
        var crossProduct = CrossProduct(other);

        var angle = Math.Acos(dotProduct / (Length * other.Length)).ToDegrees();

        var otherWay = crossProduct < 0;
        return otherWay ? (360 - angle) : angle;
    }

    public double DotProduct(HaloVector other)
    {
        return (X * other.X) + (Y * other.Y);
    }

    public double CrossProduct(HaloVector other)
    {
        return (Y * other.X) - (X * other.Y);
    }
}
