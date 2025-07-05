using System.Collections.ObjectModel;

namespace DevWinUIGallery.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    #region SwitchPresenter
    [ObservableProperty]
    public partial RadioButton RadioSelectedItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Animal> SwitchPresenterItems { get; set; } = new ObservableCollection<Animal>(Enum.GetValues<Animal>());

    #endregion

    #region Picker

    [ObservableProperty]
    public partial ObservableCollection<Windows.Storage.Pickers.PickerLocationId> PickerLocationItems { get; set; } = new ObservableCollection<Windows.Storage.Pickers.PickerLocationId>(Enum.GetValues<Windows.Storage.Pickers.PickerLocationId>());

    [ObservableProperty]
    public partial object SuggestedStartLocationSelectedItem { get; set; }

    #endregion

    #region KeyVisual
    [ObservableProperty]
    public partial ObservableCollection<VisualType> VisualTypeItems { get; set; } = new ObservableCollection<VisualType>(Enum.GetValues<VisualType>());
    #endregion

    #region AnimatedImage
    private DispatcherTimer _timer;
    private readonly Random _random = new();

    private List<string> imagePaths { get; set; } = new List<string>
    {
        "ms-appx:///Assets/Others/Girl.jpg",
        "ms-appx:///Assets/Others/GirlAfter.png",
        "ms-appx:///Assets/Others/GirlBefore.png",
        "ms-appx:///Assets/Others/GirlBlur.jpg",
        "ms-appx:///Assets/Others/Profile.png"
    };

    [ObservableProperty]
    public partial string ImageUrl { get; set; } = "ms-appx:///Assets/Others/Girl.jpg";

    public void InitAnimatedImageTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        _timer.Tick -= Timer_Tick;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }
    private void Timer_Tick(object sender, object e)
    {
        if (imagePaths.Count > 0)
        {
            var index = _random.Next(imagePaths.Count);
            ImageUrl = imagePaths[index];
        }
    }
    public void StopTimer()
    {
        _timer?.Stop();
        _timer = null;
    }
    #endregion
}
