namespace DevWinUI;

public static partial class ColorAnalyzerExtensions
{
    /// <summary>
    /// Extends the list of colors to ensure it meets the minimum count by repeating the <paramref name="index"/>th color.
    /// </summary>
    /// <param name="colors">The list of colors to extend</param>
    /// <param name="minCount">The minimum number of colors required</param>
    /// <param name="index">The index of the item to repeat</param>
    public static List<Color> EnsureMinColorCount(this List<Color> colors, int minCount, int index = 0)
    {
        // If we already have enough colors, do nothing.
        if (colors.Count >= minCount)
            return colors;

        var nthColor = colors[index];
        while (colors.Count < minCount)
        {
            colors.Add(nthColor);
        }

        return colors;
    }
}
