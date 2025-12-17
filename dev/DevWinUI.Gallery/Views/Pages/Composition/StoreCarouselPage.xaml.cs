namespace DevWinUIGallery.Views;

public sealed partial class StoreCarouselPage : Page
{
    public BaseViewModel ViewModel { get; }
    public StoreCarouselPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
        ViewModel.GenerateStoreCarouselData();

        foreach (var item in ViewModel.StoreCarouselData)
        {
            item.ActionButtonClick -= OnActionButtonClick;
            item.ActionButtonClick += OnActionButtonClick;
        }
    }

    private async void OnActionButtonClick(object sender, StoreCarouselEventArgs e)
    {
        await MessageBox.ShowInfoAsync("Action Button Clicked!");
    }

    private async void StoreCarouselSample_ItemClick(object sender, StoreCarouselEventArgs e)
    {
        await MessageBox.ShowInfoAsync("Item Clicked!");
    }
}
