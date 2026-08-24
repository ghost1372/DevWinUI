namespace DevWinUI_Template.Models;

public class PackageModeModel : BaseModel
{
    private object? _icon;
    public object? Icon
    {
        get => _icon;
        set => SetField(ref _icon, value);
    }

    private string? description;
    public string? Description
    {
        get => description;
        set => SetField(ref description, value);
    }
}
