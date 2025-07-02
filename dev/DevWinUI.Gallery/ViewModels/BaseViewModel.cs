using System.Collections.ObjectModel;

namespace DevWinUIGallery.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    #region SwitchPresenter
    [ObservableProperty]
    public partial RadioButton RadioSelectedItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Animal> SwitchPresenterItems { get; set; } = new ObservableCollection<Animal>(Enum.GetValues<Animal>());

    #endregion

    #region Picker

    [ObservableProperty]
    public partial ObservableCollection<Windows.Storage.Pickers.PickerLocationId> PickerLocationItems { get; set; } = new ObservableCollection<Windows.Storage.Pickers.PickerLocationId>(Enum.GetValues<Windows.Storage.Pickers.PickerLocationId>());

    [ObservableProperty]
    public partial object SuggestedStartLocationSelectedItem { get; set; }

    #endregion

    #region KeyVisual
    [ObservableProperty]
    public partial ObservableCollection<VisualType> VisualTypeItems { get; set; } = new ObservableCollection<VisualType>(Enum.GetValues<VisualType>());
    #endregion
}
