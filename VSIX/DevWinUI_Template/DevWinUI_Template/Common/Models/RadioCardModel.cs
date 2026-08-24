namespace DevWinUI_Template.Models;

public class RadioCardModel : BaseModel
{
    public RadioCardModel()
    {
        
    }
    public RadioCardModel(string value)
    {
        Value = value;
    }

    private RadioCardSeverity? _severity;
    public RadioCardSeverity? Severity
    {
        get => _severity;
        set => SetField(ref _severity, value);
    }
}