namespace DevWinUIGallery.Models;

public partial class ColorFormatModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string Example { get; set; }

    [ObservableProperty]
    public partial bool IsShown { get; set; }

    [ObservableProperty]
    public partial bool CanMoveUp { get; set; } = true;

    [ObservableProperty]
    public partial bool CanMoveDown { get; set; } = true;

    public ColorFormatModel(string name, string example, bool isShown)
    {
        Name = name;
        Example = example;
        IsShown = isShown;
    }
}
