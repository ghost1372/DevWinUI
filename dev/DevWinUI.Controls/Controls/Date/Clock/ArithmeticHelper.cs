using Windows.Foundation;

namespace DevWinUI;
internal class ArithmeticHelper
{
    public static double CalAngle(Point center, Point p) => Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI;
}
