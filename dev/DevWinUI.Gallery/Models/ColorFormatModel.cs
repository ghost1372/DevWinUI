namespace DevWinUIGallery.Models;

public partial class ColorFormatModel : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _example;

    [ObservableProperty]
    private bool _isShown;

    [ObservableProperty]
    private bool _canMoveUp = true;

    [ObservableProperty]
    private bool _canMoveDown = true;

    public ColorFormatModel(string name, string example, bool isShown)
    {
        Name = name;
        Example = example;
        IsShown = isShown;
    }    
}
