using System.Collections.ObjectModel;
using DevWinUIGallery.Models;

namespace DevWinUIGallery.Views;

public sealed partial class ImageResizerPage : Page
{
    public ObservableCollection<ImageSize> Sizes { get; set; } = new ObservableCollection<ImageSize>();
    public ImageResizerPage()
    {
        this.InitializeComponent();

        Sizes.Add(new ImageSize { Name = "Small", Fit = 100, Width = 854, Unit = 100, Height = 480, Id = 0 });
        Sizes.Add(new ImageSize { Name = "Medium", Fit = 100, Width = 1366, Unit = 100, Height = 768, Id = 1 });
        Sizes.Add(new ImageSize { Name = "Large", Fit = 100, Width = 1920, Unit = 100, Height = 1080, Id = 2 });
        Sizes.Add(new ImageSize { Name = "Phone", Fit = 100, Width = 320, Unit = 100, Height = 568, Id = 3 });
    }
}
