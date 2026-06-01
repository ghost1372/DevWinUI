// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal static partial class CanvasTextLayoutExtensions
{
    extension(CanvasTextLayout? canvasTextLayout)
    {
        public void SetFontFamily(string? text, string cjk, string latin)
        {
            if (canvasTextLayout == null) return;
            if (text == null) return;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                bool isCJK = c >= 0x3000 && c <= 0x9FFF   // CJK Unified Ideographs, Hiragana, Katakana
                          || c >= 0xF900 && c <= 0xFAFF    // CJK Compatibility Ideographs
                          || c >= 0xAC00 && c <= 0xD7AF;   // Hangul Syllables

                canvasTextLayout.SetFontFamily(i, 1, isCJK ? cjk : latin);
            }
        }
    }
}
