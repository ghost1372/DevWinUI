// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal static partial class LyricsAlignmentTypeExtensions
{
    public static HorizontalAlignment ToHorizontalAlignment(this LyricsTextAlignmentType alignmentType)
    {
        return alignmentType switch
        {
            LyricsTextAlignmentType.Left => HorizontalAlignment.Left,
            LyricsTextAlignmentType.Center => HorizontalAlignment.Center,
            LyricsTextAlignmentType.Right => HorizontalAlignment.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(alignmentType), alignmentType, null),
        };
    }

    public static CanvasHorizontalAlignment ToCanvasHorizontalAlignment(this LyricsTextAlignmentType alignmentType)
    {
        return alignmentType switch
        {
            LyricsTextAlignmentType.Left => CanvasHorizontalAlignment.Left,
            LyricsTextAlignmentType.Center => CanvasHorizontalAlignment.Center,
            LyricsTextAlignmentType.Right => CanvasHorizontalAlignment.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(alignmentType), alignmentType, null),
        };
    }
}
