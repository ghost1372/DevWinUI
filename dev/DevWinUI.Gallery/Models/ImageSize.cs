namespace DevWinUIGallery.Models;

public partial class ImageSize : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private int _fit;

    [ObservableProperty]
    private double _height;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private int _unit;
}
