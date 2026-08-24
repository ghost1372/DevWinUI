using System;

namespace DevWinUI_Template.Models;

public class NavigationMenuModel : BaseModel
{
    private string? _name;
    public string? Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }
    private int _stepNumber;
    public int StepNumber
    {
        get => _stepNumber;
        set => SetField(ref _stepNumber, value);
    }
    private bool _isCompleted;
    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetField(ref _isCompleted, value);
    }
    private bool _isCurrent;
    public bool IsCurrent
    {
        get => _isCurrent;
        set => SetField(ref _isCurrent, value);
    }
    private Type? _pageType;
    public Type? PageType
    {
        get => _pageType;
        set => SetField(ref _pageType, value);
    }
}
