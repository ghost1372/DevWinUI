using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DevWinUI_Template.Models;

public class BaseModel : INotifyPropertyChanged
{
    private CornerRadius? _cornerRadius = new System.Windows.CornerRadius(4);
    public CornerRadius? CornerRadius
    {
        get => _cornerRadius;
        set => SetField(ref _cornerRadius, value);
    }
    private string? _title;
    public string? Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }
    private string? _subtitle;
    public string? Subtitle
    {
        get => _subtitle;
        set => SetField(ref _subtitle, value);
    }

    private string? _value;
    public string? Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }

    private string? _tag;
    public string? Tag
    {
        get => _tag;
        set => SetField(ref _tag, value);
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    internal void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}