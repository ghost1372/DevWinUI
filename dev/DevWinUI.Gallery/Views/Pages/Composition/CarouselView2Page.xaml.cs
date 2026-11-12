namespace DevWinUIGallery.Views;

public sealed partial class CarouselView2Page : Page
{
    public BaseViewModel ViewModel { get; }

    public CarouselView2Page()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
