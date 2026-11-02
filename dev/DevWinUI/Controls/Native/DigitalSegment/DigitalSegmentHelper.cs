namespace DevWinUI;

internal static partial class DigitalSegmentHelper
{
    public const string COLON_TOP_KEY = "ColonTop";
    public const string COLON_BOTTOM_KEY = "ColonBottom";

    private static readonly string[] AsciiStrings = CreateAsciiStrings();

    /// <summary>
    /// Returns a cached string for ASCII characters to avoid allocating on every update or scroll step.
    /// </summary>
    public static string CharToString(char c) => c < AsciiStrings.Length ? AsciiStrings[c] : c.ToString();

    private static string[] CreateAsciiStrings()
    {
        var result = new string[128];
        for (int i = 0; i < result.Length; i++)
            result[i] = ((char)i).ToString();

        return result;
    }
}
