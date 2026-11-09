namespace DevWinUI;

public sealed partial class SearchBoxDetailRow
{
    public SearchBoxDetailRow(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
    public bool ShowDivider { get; internal set; }
}
