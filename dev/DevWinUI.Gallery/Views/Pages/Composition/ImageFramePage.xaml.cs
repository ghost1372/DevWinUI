namespace DevWinUIGallery.Views;

public sealed partial class ImageFramePage : Page
{
    public BaseViewModel ViewModel { get; }
    private List<Uri> _uris;
    private int _currentIndex = 0;
    public ImageFramePage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();

        _uris = new List<Uri>()
        {
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-1.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-2.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-3.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-4.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-5.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-6.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-7.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-8.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-9.jpg"),
            new Uri($"ms-appx:///Assets/Landscapes/Landscape-10.jpg"),
        };

        ImageFrame.Source = _uris[_currentIndex];
    }


    private void BtnChange_Click(object sender, RoutedEventArgs e)
    {
        if (_currentIndex < _uris.Count - 1)
        {
            _currentIndex += 1;
        }
        else
        {
            _currentIndex = 0;
        }

        ImageFrame.Source = _uris[_currentIndex];
    }
}
