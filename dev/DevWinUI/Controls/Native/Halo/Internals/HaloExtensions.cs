//https://github.com/benthorner/radial_controls

namespace DevWinUI;

internal static partial class HaloExtensions
{
    public static double ToRadians(this double degrees)
    {
        return degrees / 180 * Math.PI;
    }

    public static double ToDegrees(this double radians)
    {
        return radians * 180 / Math.PI;
    }

    public static HaloVector RelativeTo(this Point point, Point centre)
    {
        return new HaloVector(point.X - centre.X, point.Y - centre.Y);
    }
}
