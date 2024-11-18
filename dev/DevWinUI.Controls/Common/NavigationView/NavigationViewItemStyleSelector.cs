namespace DevWinUI;
internal partial class NavigationViewItemStyleSelector : StyleSelector
{
    public Style TopLevelTemplate { get; set; }
    public Style SubLevelTemplate { get; set; }

    protected override Style SelectStyleCore(object item, DependencyObject container)
    {
        if (item == null) return TopLevelTemplate;

        var navItem = item as NavigationViewItem;
        if (navItem == null) return null;

        var subItem = VisualHelper.FindParentOfType<NavigationViewItem>(navItem);

        if (subItem == null)
        {
            return TopLevelTemplate;
        }
        else
        {

            return SubLevelTemplate;
        }
    }
}
