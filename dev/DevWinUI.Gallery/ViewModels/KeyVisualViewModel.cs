using System.Collections.ObjectModel;

namespace DevWinUIGallery.ViewModels;
public partial class KeyVisualViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<VisualType> Items { get; set; } = new ObservableCollection<VisualType>(Enum.GetValues<VisualType>());
}
