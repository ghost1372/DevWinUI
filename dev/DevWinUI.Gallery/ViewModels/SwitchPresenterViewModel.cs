using System.Collections.ObjectModel;

namespace DevWinUIGallery.ViewModels;

public partial class SwitchPresenterViewModel : ObservableObject
{
    [ObservableProperty]
    public partial RadioButton RadioSelectedItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Animal> Items { get; set; } = new ObservableCollection<Animal>(Enum.GetValues<Animal>());
}
