namespace DevWinUI;

public sealed partial class SearchResultEventArgs : EventArgs
{
    public SearchResultEventArgs(SearchResultItem? item, string query)
    {
        Item = item;
        Query = query;
    }

    public SearchResultItem? Item { get; }
    public string Query { get; }
}
