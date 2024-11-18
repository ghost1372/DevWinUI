using System.Collections.ObjectModel;

namespace DevWinUI;

public partial class DataItem : BaseDataInfo
{
    public string UniqueId { get; set; }
    public string SectionId { get; set; }
    public string Title { get; set; }
    public string SecondaryTitle { get; set; }
    public object Parameter { get; set; }
    public string ApiNamespace { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public string IconGlyph { get; set; }
    public string BadgeString { get; set; }
    public string Content { get; set; }
    public bool IsNew { get; set; }
    public bool IsUpdated { get; set; }
    public bool IsPreview { get; set; }
    public bool HideItem { get; set; }
    public bool HideNavigationViewItem { get; set; }
    public ObservableCollection<DataLink> Links { get; set; }
    public ObservableCollection<string> Extra { get; set; }
    public bool IncludedInBuild { get; set; } = true;
    public bool IsNavigationViewItemHeader { get; set; }
    public bool UsexUid { get; set; }
    public override string ToString()
    {
        return Title;
    }
}