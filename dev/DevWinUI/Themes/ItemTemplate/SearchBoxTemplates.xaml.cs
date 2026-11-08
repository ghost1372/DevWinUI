namespace DevWinUI;

public sealed partial class SearchBoxTemplates : ResourceDictionary
{
    public SearchBoxTemplates()
    {
        InitializeComponent();
    }

    private void ResultIcon_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (args.NewValue is SearchResultItem { IconBrush: { } brush })
        {
            ((FontIcon)sender).Foreground = brush;
        }
        else
        {
            sender.ClearValue(IconElement.ForegroundProperty);
        }
    }

    private void MoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: SearchResultItem item } anchor)
        {
            FindOwner(anchor)?.ShowItemMenu(anchor, item);
        }
    }

    private void OpenItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: SearchResultItem item } anchor)
        {
            FindOwner(anchor)?.InvokeItem(item);
        }
    }

    private static SearchBox? FindOwner(DependencyObject element)
    {
        var parent = VisualTreeHelper.GetParent(element);

        while (parent is not null)
        {
            if (parent is ListView { Tag: SearchBox owner })
            {
                return owner;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }
}
