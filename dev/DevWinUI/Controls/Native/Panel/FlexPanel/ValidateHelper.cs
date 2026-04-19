namespace DevWinUI;

internal class ValidateHelper
{
    public static bool IsInRangeOfPosDoubleIncludeZero(object value)
    {
        var v = (double)value;
        return !(double.IsNaN(v) || double.IsInfinity(v)) && v >= 0;
    }
}
