namespace DevWinUI;

public partial class DataItem : BaseDataInfo
{
    public object Parameter { get; set; }
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
    public override string ToString()
    {
        return Title;
    }
}
