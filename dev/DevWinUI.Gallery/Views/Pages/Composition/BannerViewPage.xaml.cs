using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public sealed partial class BannerViewPage : Page
{
    public BaseViewModel ViewModel { get; }
    public CycleCollection<Uri> CycleList { get; set; }
    public ObservableCollection<Uri> uris = new ObservableCollection<Uri>();
    public BannerViewPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();

        uris = new(ViewModel.BannerViewData.Select(x => x.ImageUri).ToList());
        CycleList = new CycleCollection<Uri>(uris);
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
