using System.Collections.ObjectModel;

namespace DevWinUIGallery.ViewModels;
public partial class PickerViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Windows.Storage.Pickers.PickerLocationId> Items { get; set; } = new ObservableCollection<Windows.Storage.Pickers.PickerLocationId>(Enum.GetValues<Windows.Storage.Pickers.PickerLocationId>());

    [ObservableProperty]
    public partial object SuggestedStartLocationSelectedItem { get; set; }
}
