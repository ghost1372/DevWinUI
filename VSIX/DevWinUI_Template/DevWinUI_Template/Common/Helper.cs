using DevWinUI_Template.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DevWinUI_Template.Common;

public static class Helper
{
    private const int BaseCornerRadius = 4;

    public static void UpdateCornerRadius(System.Windows.Controls.ListView listView)
    {
        if (listView.Items.Count == 0)
            return;

        var firstItem = listView.Items[0] as BaseModel;
        var lastItem = listView.Items[listView.Items.Count - 1] as BaseModel;

        foreach (BaseModel item in listView.Items)
        {
            if (item != firstItem && item != lastItem)
                item.CornerRadius = new CornerRadius(0);
        }

        firstItem?.CornerRadius = new CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0);
        lastItem?.CornerRadius = new CornerRadius(0, 0, BaseCornerRadius, BaseCornerRadius);
    }

    public static void UpdateActiveCornerRadius(System.Windows.Controls.ListView listView)
    {
        if (listView.Items.Count == 0)
            return;

        var items = listView.Items.Cast<BaseModel>().ToList();
        var selectedItems = listView.SelectedItems.Cast<object>().Select(x => x as BaseModel).Where(x => x != null).ToHashSet();

        foreach (var item in items)
        {
            item.CornerRadius = new CornerRadius(0);
        }

        var firstItem = items.FirstOrDefault();
        var lastItem = items.LastOrDefault();

        if (firstItem != null && !selectedItems.Contains(firstItem))
            firstItem.CornerRadius = new CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0);

        if (lastItem != null && !selectedItems.Contains(lastItem))
            lastItem.CornerRadius = new CornerRadius(0, 0, BaseCornerRadius, BaseCornerRadius);

        if (selectedItems.Count == 0)
            return;

        var selectedIndexes = new HashSet<int>();
        for (var i = 0; i < items.Count; i++)
        {
            if (selectedItems.Contains(items[i]))
                selectedIndexes.Add(i);
        }

        foreach (var selectedIndex in selectedIndexes)
        {
            var selectedItem = items[selectedIndex];
            selectedItem.CornerRadius = new CornerRadius(BaseCornerRadius);

            if (selectedIndex > 0)
            {
                var previousItem = items[selectedIndex - 1];

                if (!selectedIndexes.Contains(selectedIndex - 1) && previousItem != null)
                {
                    previousItem.CornerRadius = new CornerRadius(
                        previousItem == firstItem ? BaseCornerRadius : 0,
                        previousItem == firstItem ? BaseCornerRadius : 0,
                        BaseCornerRadius,
                        BaseCornerRadius);
                }
            }

            if (selectedIndex < items.Count - 1)
            {
                var nextItem = items[selectedIndex + 1];

                if (!selectedIndexes.Contains(selectedIndex + 1) && nextItem != null)
                {
                    nextItem.CornerRadius = new CornerRadius(
                        BaseCornerRadius,
                        BaseCornerRadius,
                        nextItem == lastItem ? BaseCornerRadius : 0,
                        nextItem == lastItem ? BaseCornerRadius : 0);
                }
            }
        }
    }
}

