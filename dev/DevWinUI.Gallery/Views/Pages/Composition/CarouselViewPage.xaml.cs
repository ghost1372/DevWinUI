namespace DevWinUIGallery.Views;

public sealed partial class CarouselViewPage : Page
{
    public BaseViewModel ViewModel { get; }

    public CarouselViewPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private async void CarouselSample_ItemClick(object sender, CarouselViewItemClickEventArgs e)
    {
        await MessageBox.ShowInfoAsync($"You have clicked {(e.ClickItem as ICarouselViewItemSource).Title} ;-)", "Wow");
    }
}

