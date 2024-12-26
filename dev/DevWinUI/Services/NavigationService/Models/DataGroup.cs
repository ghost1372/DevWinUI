using System.Collections.ObjectModel;

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
    public ObservableCollection<DataItem> Items { get; set; } = new();
    public override string ToString()
    {
        return Title;
    }
}
