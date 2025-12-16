using DevWinUIGallery.Models;

namespace DevWinUIGallery.Views;

public sealed partial class EasyCarouselPanelPage : Page
{
    public BaseViewModel ViewModel { get; }
    public EasyCarouselPanelPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private async void EasyCarouselPanelSample_ItemTapped(object sender, FrameworkElement e)
    {
        await MessageBox.ShowInfoAsync((e?.DataContext as SampleItemModel)?.Title);
    }
}
