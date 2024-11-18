using System.Collections.ObjectModel;

namespace DevWinUI;

public partial class DataGroup : BaseDataInfo
{
    public string UniqueId { get; set; }
    public string SectionId { get; set; }
    public string ApiNamespace { get; set; }
    public string Title { get; set; }
    public string SecondaryTitle { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public string IconGlyph { get; set; }
    public bool IsSpecialSection { get; set; }
    public bool IsExpanded { get; set; }
    public bool HideGroup { get; set; }
    public bool ShowItemsWithoutGroup { get; set; }
    public bool IsFooterNavigationViewItem { get; set; }
    public bool IsNavigationViewItemHeader { get; set; }
    public bool UsexUid { get; set; }
    public bool Order { get; set; }
    public bool OrderByDescending { get; set; }
    public ObservableCollection<DataItem> Items { get; set; } = new();
    public override string ToString()
    {
        return Title;
    }
}
