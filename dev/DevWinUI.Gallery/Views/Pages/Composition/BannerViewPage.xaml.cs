using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public sealed partial class BannerViewPage : Page
{
    public CycleCollection<Uri> CycleList { get; set; }
    private ObservableCollection<Uri> list = new ObservableCollection<Uri>();
    public BannerViewPage()
    {
        InitializeComponent();

        list.Add(new Uri("ms-appx:///Assets/BannerView/1.jpeg"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/2.png"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/3.jpeg"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/4.jpeg"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/5.jpeg"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/6.png"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/7.jpeg"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/8.jpeg"));
        list.Add(new Uri("ms-appx:///Assets/BannerView/9.jpeg"));

        CycleList = new CycleCollection<Uri>(list);
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
