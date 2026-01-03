using System.ComponentModel;

namespace DevWinUIGallery.Models;

public sealed partial class FolderItem : ObservableObject, ISidebarItemModel
{
    [ObservableProperty]
    public partial string FolderText { get; set; } = "";

    [ObservableProperty]
    public partial string Path { get; set; } = "";

    [ObservableProperty]
    public partial ImageIconSource Icon { get; set; } = new ImageIconSource();

    public object? Children { get; set; } = null;

    public IconSource? IconSource => Icon;

    public bool IsExpanded { get; set; } = false;

    public object ToolTip => Path;

    public bool PaddedItem => false;

    public string Text => FolderText;

    partial void OnIconChanged(ImageIconSource value)
    {
        // Notify that IconSource also changed when Icon changes
        OnPropertyChanged(nameof(IconSource));
    }
}
