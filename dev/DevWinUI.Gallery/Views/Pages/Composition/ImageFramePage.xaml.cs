using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class ImageFramePage : Page
{
    public ObservableCollection<ImageFrameTransitionMode> ImageFrameTransition { get; set; } = new ObservableCollection<ImageFrameTransitionMode>(Enum.GetValues<ImageFrameTransitionMode>());

    public ObservableCollection<AlignmentX> ImageFrameAlignmentX { get; set; } = new ObservableCollection<AlignmentX>(Enum.GetValues<AlignmentX>());

    public ObservableCollection<AlignmentY> ImageFrameAlignmentY { get; set; } = new ObservableCollection<AlignmentY>(Enum.GetValues<AlignmentY>());

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
