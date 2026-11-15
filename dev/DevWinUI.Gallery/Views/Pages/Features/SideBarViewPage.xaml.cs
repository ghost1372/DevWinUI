namespace DevWinUIGallery.Views;

public sealed partial class SideBarViewPage : Page
{
    public ObservableCollection<SidebarItemModel> Items { get; } = new()
    {
        new SidebarItemModel { Text = "Home", Path = "C:\\Fonts", IconImage = "Home" },
        new SidebarItemModel
        {
            Text = "Folder",
            Children = new()
            {
                new SidebarItemModel { Text = "SubFolder1", Path = "C:\\Folder\\SubFolder1", IconImage = "Folder" },
                new SidebarItemModel { Text = "SubFolder2", Path = "C:\\Folder\\SubFolder2", IconImage = "Folder" }
            }
        },
        new SidebarItemModel { Text = "Drive", Path = "D:\\Drive", IconImage = "CloudDrive" }
    };

    public SidebarViewMenuSource SidebarViewItems { get; }
    public SideBarViewPage()
    {
        InitializeComponent();
        SidebarViewItems = new SidebarViewMenuSource(Items);
    }
}
