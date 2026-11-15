namespace DevWinUIGallery.Views;

public sealed partial class SideBarViewPage : Page
{
    public ObservableCollection<SidebarViewSampleItem> Items { get; } = new()
    {
        new SidebarViewSampleItem { Text = "Home", Path = "C:\\Fonts", IconImage = "Home" },
        new SidebarViewSampleItem
        {
            Text = "Folder",
            Children = new()
            {
                new SidebarViewSampleItem { Text = "SubFolder1", Path = "C:\\Folder\\SubFolder1", IconImage = "Folder" },
                new SidebarViewSampleItem { Text = "SubFolder2", Path = "C:\\Folder\\SubFolder2", IconImage = "Folder" }
            }
        },
        new SidebarViewSampleItem { Text = "Drive", Path = "D:\\Drive", IconImage = "CloudDrive" }
    };

    public SidebarViewMenuSource SidebarViewItems { get; }
    public SideBarViewPage()
    {
        InitializeComponent();
        SidebarViewItems = new SidebarViewMenuSource(Items);
    }
}
