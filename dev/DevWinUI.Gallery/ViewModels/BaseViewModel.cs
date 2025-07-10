using System.Collections.ObjectModel;
using DevWinUIGallery.Models;

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

    #region PerspectiveZoom
    [ObservableProperty]
    public partial ObservableCollection<SampleData> SamplePerspectiveData { get; set; } = new ObservableCollection<SampleData>()
    {
        new SampleData("Sample 1", "ms-appx:///Assets/Landscapes/Landscape-1.jpg", "Sample Desc 1"),
        new SampleData("Sample 2", "ms-appx:///Assets/Landscapes/Landscape-2.jpg", "Sample Desc 2"),
        new SampleData("Sample 3", "ms-appx:///Assets/Landscapes/Landscape-3.jpg", "Sample Desc 3"),
        new SampleData("Sample 4", "ms-appx:///Assets/Landscapes/Landscape-4.jpg", "Sample Desc 4"),
        new SampleData("Sample 5", "ms-appx:///Assets/Landscapes/Landscape-5.jpg", "Sample Desc 5"),
        new SampleData("Sample 6", "ms-appx:///Assets/Landscapes/Landscape-6.jpg", "Sample Desc 6"),
        new SampleData("Sample 7", "ms-appx:///Assets/Landscapes/Landscape-7.jpg", "Sample Desc 7"),
        new SampleData("Sample 8", "ms-appx:///Assets/Landscapes/Landscape-8.jpg", "Sample Desc 8"),
        new SampleData("Sample 9", "ms-appx:///Assets/Landscapes/Landscape-9.jpg", "Sample Desc 9"),
        new SampleData("Sample 10", "ms-appx:///Assets/Landscapes/Landscape-10.jpg", "Sample Desc 10"),
        new SampleData("Sample 11", "ms-appx:///Assets/Landscapes/Landscape-11.jpg", "Sample Desc 11"),
        new SampleData("Sample 12", "ms-appx:///Assets/Landscapes/Landscape-12.jpg", "Sample Desc 12"),
        new SampleData("Sample 13", "ms-appx:///Assets/Landscapes/Landscape-13.jpg", "Sample Desc 13")
    };

    #endregion
}
