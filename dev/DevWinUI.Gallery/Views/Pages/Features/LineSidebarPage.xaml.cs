namespace DevWinUIGallery.Views;

public sealed partial class LineSidebarPage : Page
{
    public IList<string> Items { get; } = new List<string>
    {
        "Overview",
        "Features",
        "Pricing",
        "Documentation",
        "Contact",
    };

    public ObservableCollection<LineSidebarFalloff> Falloffs { get; set; } = new ObservableCollection<LineSidebarFalloff>(Enum.GetValues<LineSidebarFalloff>());

    public LineSidebarPage()
    {
        InitializeComponent();
    }

    private void OnItemClick(LineSidebar sender, LineSidebarItemClickEventArgs e)
    {
        TxtClicked.Text = $"Clicked: {e.Label} (index {e.Index})";
    }
}
