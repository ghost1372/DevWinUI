// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public partial class LyricLine : BaseLyric
{
    public List<BaseLyric> PrimarySyllables { get; set; } = [];
    public List<BaseLyric> SecondarySyllables { get; set; } = [];
    public List<BaseLyric> TertiarySyllables { get; set; } = [];

    public List<BaseLyric> PrimaryChars { get; private set; } = [];
    public List<BaseLyric> SecondaryChars { get; private set; } = [];
    public List<BaseLyric> TertiaryChars { get; private set; } = [];

    public string PrimaryText { get; set; } = "";
    public string SecondaryText { get; set; } = "";
    public string TertiaryText { get; set; } = "";

    public new string Text => PrimaryText;
    public new int StartIndex = 0;

    public bool IsPrimaryHasRealSyllableInfo { get; set; } = false;

    public LyricLine()
    {
        for (int charStartIndex = 0; charStartIndex < PrimaryText.Length; charStartIndex++)
        {
            var syllable = PrimarySyllables.FirstOrDefault(x => x.StartIndex <= charStartIndex && charStartIndex <= x.EndIndex);
            if (syllable == null) continue;

            var avgCharDuration = syllable.DurationMs / syllable.Length;
            if (avgCharDuration == 0) continue;

            var charStartMs = syllable.StartMs + (charStartIndex - syllable.StartIndex) * avgCharDuration;
            var charEndMs = charStartMs + avgCharDuration;

            PrimaryChars.Add(new BaseLyric
            {
                StartIndex = charStartIndex,
                StartMs = charStartMs,
                EndMs = charEndMs,
                Text = PrimaryText[charStartIndex].ToString()
            });
        }
    }

}
