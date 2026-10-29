namespace DevWinUI;

internal static partial class ScrollSplitCardMath
{
    internal static double Phase(double p, double from, double to) => Math.Clamp((p - from) / (to - from), 0, 1);

    internal static double Smooth(double t) => t * t * (3 - 2 * t);
}
