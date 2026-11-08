using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI;

public sealed partial class SearchResultItem : INotifyPropertyChanged
{
    private bool _isSelected;
    private IList<SearchBoxDetailRow> _details = new List<SearchBoxDetailRow>();

    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Glyph { get; set; } = "\uE8A5";
    public Brush? IconBrush { get; set; }
    public ImageSource? Preview { get; set; }
    public IList<SearchBoxDetailRow> Details
    {
        get => _details;
        set
        {
            _details = value;

            for (var i = 0; i < value.Count; i++)
            {
                value[i].ShowDivider = i > 0;
            }
        }
    }
    public object? Payload { get; set; }
    public IList<SearchMenuItem> MenuItems { get; set; } = new List<SearchMenuItem>();

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowDetails));
        }
    }

    public Visibility ShowDetails =>
        _isSelected && (Preview != null || Details.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

    public event PropertyChangedEventHandler? PropertyChanged;

    public override string ToString() => Title;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
