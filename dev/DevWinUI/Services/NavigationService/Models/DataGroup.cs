namespace DevWinUI;

public partial class DataGroup : BaseDataInfo
{
    public bool IsSpecialSection { get; set; }
    public bool IsExpanded { get; set; }
    public bool HideGroup { get; set; }
    public bool ShowItemsWithoutGroup { get; set; }
    public bool IsFooterNavigationViewItem { get; set; }
    public bool Order { get; set; }
    public bool OrderByDescending { get; set; }
    public bool UseBuiltInNavigationViewInfoBadgeStyle { get; set; }
    public bool UseBuiltInLandingPageInfoBadgeStyle { get; set; } = true;
    public string DefaultBuiltInNavigationViewInfoBadgeStyle { get; set; } = "StringInfoBadgeStyle";
    public string DefaultBuiltInLandingPageInfoBadgeStyle { get; set; } = "AttentionIconInfoBadgeStyle";
    public ObservableCollection<DataItem> Items { get; set; } = new();
    public override string ToString()
    {
        return Title;
    }
}
