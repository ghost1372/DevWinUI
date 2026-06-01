// https://github.com/jayfunc/BetterLyrics

using Microsoft.UI.Text;
using Windows.UI.Text;

namespace DevWinUI;

internal static partial class LyricsFontWeightExtensions
{
    public static FontWeight ToFontWeight(this LyricFontWeight weight)
    {
        return weight switch
        {
            LyricFontWeight.Thin => FontWeights.Thin,
            LyricFontWeight.ExtraLight => FontWeights.ExtraLight,
            LyricFontWeight.Light => FontWeights.Light,
            LyricFontWeight.SemiLight => FontWeights.SemiLight,
            LyricFontWeight.Normal => FontWeights.Normal,
            LyricFontWeight.Medium => FontWeights.Medium,
            LyricFontWeight.SemiBold => FontWeights.SemiBold,
            LyricFontWeight.Bold => FontWeights.Bold,
            LyricFontWeight.ExtraBold => FontWeights.ExtraBold,
            LyricFontWeight.Black => FontWeights.Black,
            LyricFontWeight.ExtraBlack => FontWeights.ExtraBlack,
            LyricFontWeight _ => throw new ArgumentOutOfRangeException(nameof(weight), weight, null),
        };
    }
}
