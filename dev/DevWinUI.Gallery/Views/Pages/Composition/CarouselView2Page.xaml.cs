namespace DevWinUIGallery.Views;

public sealed partial class CarouselView2Page : Page
{
    public ObservableCollection<CarouselView2WheelAlignments> WheelAlignmentList { get; set; } = new ObservableCollection<CarouselView2WheelAlignments>(Enum.GetValues<CarouselView2WheelAlignments>());
    public ObservableCollection<CarouselView2CarouselTypes> CarouselTypeList { get; set; } = new ObservableCollection<CarouselView2CarouselTypes>(Enum.GetValues<CarouselView2CarouselTypes>());

    public BaseViewModel ViewModel { get; }
    public CarouselView2Page()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
