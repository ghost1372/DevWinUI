namespace DevWinUI;

public interface ISegmentPattern
{
    /// <summary>
    /// Returns a dictionary of all patterns for all characters.
    /// Key: character (as string)
    /// Value: pattern for that character
    /// Each pattern must be characters of '1' or '0'
    /// </summary>
    static abstract Dictionary<string, string> Patterns { get; }

    /// <summary>
    /// Default Pattern to use if character not found. Must be characters of '1' or '0'.
    /// </summary>
    static abstract string DefaultPattern { get; }
}
