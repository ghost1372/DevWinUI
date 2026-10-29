namespace DevWinUIGallery.Views;

public sealed partial class TableOfContentsPage : Page
{
    public ObservableCollection<TableOfContentsKind> KindItems { get; set; } = new ObservableCollection<TableOfContentsKind>(Enum.GetValues<TableOfContentsKind>());

    public IReadOnlyList<TableOfContentsItem> TocItems { get; } = new TableOfContentsItem[]
    {
        new("intro", "Introduction"),
        new("install", "Installation"),
        new("requirements", "Requirements", 3),
        new("usage", "Usage"),
        new("basic", "Basic usage", 3),
        new("advanced", "Advanced usage", 3),
        new("faq", "FAQ"),
    };

    public TableOfContentsPage()
    {
        InitializeComponent();
    }

    private void Sample_ActiveChanged(TableOfContents sender, IReadOnlyList<string> ids)
    {
        StatusText.Text = ids.Count == 0 ? "Active: none" : $"Active: {string.Join(", ", ids)}";
    }

    private void Sample_ItemInvoked(TableOfContents sender, string id)
    {
        StatusText.Text = $"Invoked: {id}";
    }
}
