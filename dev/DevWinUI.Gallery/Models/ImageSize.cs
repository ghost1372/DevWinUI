namespace DevWinUIGallery.Models;

public partial class ImageSize : ObservableObject
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial int Fit { get; set; }

    [ObservableProperty]
    public partial double Height { get; set; }

    [ObservableProperty]
    public partial double Width { get; set; }

    [ObservableProperty]
    public partial int Unit { get; set; }
}
