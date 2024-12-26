namespace DevWinUI;
public partial class BaseDataInfo
{
    public string UniqueId { get; set; }
    public string SectionId { get; set; }
    public string Title { get; set; }
    public string ApiNamespace { get; set; }
    public string SecondaryTitle { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public string IconGlyph { get; set; }
    public bool IsNavigationViewItemHeader { get; set; }
    public bool UsexUid { get; set; }
    public DataInfoBadge DataInfoBadge { get; set; }
}
