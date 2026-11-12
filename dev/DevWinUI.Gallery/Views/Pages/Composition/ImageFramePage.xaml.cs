namespace DevWinUIGallery.Views;

public sealed partial class ImageFramePage : Page
{
    public BaseViewModel ViewModel { get; }
    private int _currentIndex = 0;

    public ImageFramePage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
        ImageFrame.Source = ViewModel.LandscapeData[_currentIndex].ImagePath;
    }

    private void BtnChange_Click(object sender, RoutedEventArgs e)
    {
        if (_currentIndex < ViewModel.LandscapeData.Count - 1)
        {
            _currentIndex += 1;
        }
        else
        {
            _currentIndex = 0;
        }

        ImageFrame.Source = ViewModel.LandscapeData[_currentIndex].ImagePath;
    }
}
