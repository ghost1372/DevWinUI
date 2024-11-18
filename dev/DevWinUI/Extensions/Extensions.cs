namespace DevWinUI;
public static partial class Extensions
{
    public static SolidColorBrush GetSolidColorBrush(this string hex)
    {
        return ColorHelper.GetSolidColorBrush(hex);
    }
}
