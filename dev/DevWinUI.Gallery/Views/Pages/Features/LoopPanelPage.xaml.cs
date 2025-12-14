using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public sealed partial class LoopPanelPage : Page
{
    public ObservableCollection<LoopPanelItem> Items = new ObservableCollection<LoopPanelItem>()
    {
        new LoopPanelItem() {Title="Item 1", ImageSource = "ms-appx:///Assets/BannerView/1.jpeg"},
        new LoopPanelItem() {Title="Item 2", ImageSource = "ms-appx:///Assets/BannerView/2.png"},
        new LoopPanelItem() {Title="Item 3", ImageSource = "ms-appx:///Assets/BannerView/3.jpeg"},
        new LoopPanelItem() {Title="Item 4", ImageSource = "ms-appx:///Assets/BannerView/4.jpeg"},
        new LoopPanelItem() {Title="Item 5", ImageSource = "ms-appx:///Assets/BannerView/5.jpeg"},
        new LoopPanelItem() {Title="Item 6", ImageSource = "ms-appx:///Assets/BannerView/6.png"},
        new LoopPanelItem() {Title="Item 7", ImageSource = "ms-appx:///Assets/BannerView/7.jpeg"},
        new LoopPanelItem() {Title="Item 8", ImageSource = "ms-appx:///Assets/BannerView/8.jpeg"},
        new LoopPanelItem() {Title="Item 9", ImageSource = "ms-appx:///Assets/BannerView/9.jpeg"},
    };
    public LoopPanelPage()
    {
        InitializeComponent();
    }
}
public partial class LoopPanelItem
{
    public string ImageSource { get; set; }
    public string Title { get; set; }
}
