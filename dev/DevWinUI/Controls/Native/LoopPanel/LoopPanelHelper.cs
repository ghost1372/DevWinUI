namespace DevWinUI;

internal partial class LoopPanelHelper
{
    public static bool AreVirtuallyEqual(double d1, double d2)
    {
        if (double.IsPositiveInfinity(d1))
            return double.IsPositiveInfinity(d2);

        if (double.IsNegativeInfinity(d1))
            return double.IsNegativeInfinity(d2);

        if (double.IsNaN(d1))
            return double.IsNaN(d2);

        double n = d1 - d2;
        double d = (Math.Abs(d1) + Math.Abs(d2) + 10) * 1.0e-15;
        return (-d < n) && (d > n);
    }

    public static bool AreVirtuallyEqual(Size s1, Size s2)
    {
        return (AreVirtuallyEqual(s1.Width, s2.Width)
            && AreVirtuallyEqual(s1.Height, s2.Height));
    }

    public static bool AreVirtuallyEqual(Point p1, Point p2)
    {
        return (AreVirtuallyEqual(p1.X, p2.X)
            && AreVirtuallyEqual(p1.Y, p2.Y));
    }

    public static bool AreVirtuallyEqual(Rect r1, Rect r2)
    {
        return AreVirtuallyEqual(new Point(r1.X, r1.Y), new Point(r2.X, r2.Y))
            && AreVirtuallyEqual(new Point(r1.X + r1.Width, r1.Y + r1.Height),
                                 new Point(r2.X + r2.Width, r2.Y + r2.Height));
    }

    public static bool GreaterThanOrVirtuallyEqual(double d1, double d2)
    {
        return (d1 > d2 || AreVirtuallyEqual(d1, d2));
    }

    public static bool LessThanOrVirtuallyEqual(double d1, double d2)
    {
        return (d1 < d2 || AreVirtuallyEqual(d1, d2));
    }

    public static bool StrictlyLessThan(double d1, double d2)
    {
        return (d1 < d2 && !AreVirtuallyEqual(d1, d2));
    }

    public static bool StrictlyGreaterThan(double d1, double d2)
    {
        return (d1 > d2 && !AreVirtuallyEqual(d1, d2));
    }
}
