namespace DevWinUI_Template.Models;

public class CheckCardModel : BaseModel
{
    private object? _icon;
    public object? Icon
    {
        get => _icon;
        set => SetField(ref _icon, value);
    }

    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetField(ref _isEnabled, value);
    }
}