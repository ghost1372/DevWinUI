namespace DevWinUIGallery;

internal static class SelectorBarItemsExtensions
{
    public static int ItemCount(this SelectorBar bar)
    {
        var count = 0;
        foreach (var _ in bar.Items)
        {
            count++;
        }

        return count;
    }

    public static void ClearItems(this SelectorBar bar)
    {
        var items = bar.Items;
        for (var i = bar.ItemCount() - 1; i >= 0; i--)
        {
            items.RemoveAt(i);
        }
    }

    public static void AddItem(this SelectorBar bar, SelectorBarItem item)
    {
        bar.Items.Insert(bar.ItemCount(), item);
    }

    public static SelectorBarItem FindItem(this SelectorBar bar, Func<SelectorBarItem, bool> predicate)
    {
        foreach (var item in bar.Items)
        {
            if (predicate(item))
            {
                return item;
            }
        }

        return null;
    }
}
