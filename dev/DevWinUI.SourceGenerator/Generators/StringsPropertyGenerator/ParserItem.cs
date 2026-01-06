using System;

namespace DevWinUI;

internal partial class ParserItem
{
    private string _key = string.Empty;

    internal string Key
    {
        get => _key;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Key cannot be null or empty.");
            _key = value;
        }
    }

    internal string Value { get; set; } = string.Empty;

    internal string? Comment { get; set; }
}
