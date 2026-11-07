namespace DevWinUI;

internal static class MathHelper
{
    internal const double DBL_EPSILON = 2.2204460492503131e-016;
    public static bool AreClose(double value1, double value2) =>
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        value1 == value2 || IsVerySmall(value1 - value2);

    public static double Lerp(double x, double y, double alpha) => x * (1.0 - alpha) + y * alpha;

    public static bool IsVerySmall(double value) => Math.Abs(value) < 1E-06;
    public static bool GreaterThan(double value1, double value2) => value1 > value2 && !AreClose(value1, value2);

    public static bool GreaterThanOrClose(double value1, double value2)
    {
        if (value1 <= value2)
        {
            return AreClose(value1, value2);
        }
        return true;
    }

    public static bool IsZero(double value) => Math.Abs(value) < 10.0 * DBL_EPSILON;
}
