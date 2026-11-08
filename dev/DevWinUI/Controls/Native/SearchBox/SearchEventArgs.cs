namespace DevWinUI;

public sealed partial class SearchEventArgs : EventArgs
{
    public SearchEventArgs(string query) => Query = query;
    public string Query { get; }
}
