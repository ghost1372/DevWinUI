namespace DevWinUI;

public sealed partial class SearchMenuItemEventArgs : EventArgs
{
    public SearchMenuItemEventArgs(SearchResultItem item, SearchMenuItem menuItem)
    {
        Item = item;
        MenuItem = menuItem;
    }

    public SearchResultItem Item { get; }
    public SearchMenuItem MenuItem { get; }
}
