using BreadcrumbBarItem = DevWinUI.BreadcrumbBarItem;

namespace DevWinUIGallery.Views;

public sealed partial class BreadcrumbBarPage : Page
{
    public ObservableCollection<BreadcrumbBarItem> Items { get; set; } = new ObservableCollection<BreadcrumbBarItem>
    {
        new BreadcrumbBarItem { Content = "Home" },
        new BreadcrumbBarItem { Content = "Documents" },
        new BreadcrumbBarItem { Content = "Projects" },
        new BreadcrumbBarItem { Content = "2026" }
    };
    public BreadcrumbBarPage()
    {
        InitializeComponent();
    }
}
