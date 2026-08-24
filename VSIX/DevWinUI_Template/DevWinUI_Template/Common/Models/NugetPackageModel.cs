namespace DevWinUI_Template.Models;

public class NugetPackageModel : BaseModel
{
    public NugetPackageModel()
    {
        
    }
    public NugetPackageModel(string packageName)
    {
        this.PackageName = packageName;
    }
    private string? _packageName;
    public string? PackageName
    {
        get => _packageName;
        set => SetField(ref _packageName, value);
    }
    private bool _HasImplementation;
    public bool HasImplementation
    {
        get => _HasImplementation;
        set => SetField(ref _HasImplementation, value);
    }
}
