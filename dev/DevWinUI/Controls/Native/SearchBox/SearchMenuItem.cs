namespace DevWinUI;

public sealed partial class SearchMenuItem
{
    public string Text { get; set; } = string.Empty;
    public IconSource? IconSource { get; set; }
    public object? Tag { get; set; }
    public bool IsSeparator { get; set; }
    public Action<SearchResultItem>? Action { get; set; }
}
