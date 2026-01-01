using System.ComponentModel;

namespace DevWinUIGallery.Models;

public sealed partial class FolderItem : ISidebarItemModel, INotifyPropertyChanged
{
    private string text = "";
    public string Text
    {
        get => text;
        set => SetProperty(ref text, value, nameof(Text));
    }

    private string path = "";
    public string Path
    {
        get => path;
        set => SetProperty(ref path, value, nameof(Path));
    }

    private ImageIconSource icon = new ImageIconSource();
    public ImageIconSource Icon
    {
        get => icon;
        set
        {
            SetProperty(ref icon, value, nameof(Icon));
            OnPropertyChanged(nameof(IconSource));
        }
    }

    public object? Children { get; set; } = null;

    public IconSource? IconSource => Icon;

    public bool IsExpanded { get; set; } = false;

    public object ToolTip => Path;

    public bool PaddedItem => false;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetProperty<T>(ref T storage, T value, string propertyName)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
