using System.Runtime.CompilerServices;

namespace DevWinUI;

internal static partial class DoubleExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFinite(this double value)
    {
        return !double.IsInfinity(value) && !double.IsNaN(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFinite(this float value)
    {
        return !float.IsInfinity(value) && !float.IsNaN(value);
    }
}
