using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public sealed partial class BannerViewPage : Page
{
    public BaseViewModel ViewModel { get; }
    public CycleCollection<Uri> CycleList { get; }

    public BannerViewPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        CycleList = new CycleCollection<Uri>(new ObservableCollection<Uri>(ViewModel.BannerViewData.Select(x => x.ImageUri)));
        InitializeComponent();
    }

    private void BtnPlayBackward_Click(object sender, RoutedEventArgs e)
    {
        BannerViewSample.PlayShuffleBackward();
    }

    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        BannerViewSample.StopShuffle();
    }

    private void BtnPlayForward_Click(object sender, RoutedEventArgs e)
    {
        BannerViewSample.PlayShuffleForward();
    }
}
