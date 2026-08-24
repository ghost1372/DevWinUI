namespace DevWinUI_Template.Models;

public class TileCardModel : BaseModel
{
    private object? _icon;
    public object? Icon
    {
        get => _icon;
        set => SetField(ref _icon, value);
    }
}
