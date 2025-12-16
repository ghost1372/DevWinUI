namespace DevWinUIGallery.Views;

public sealed partial class ContentSliderPage : Page
{
    public BaseViewModel ViewModel { get; }
    public ContentSliderPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
