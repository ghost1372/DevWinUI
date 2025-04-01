using System.Collections.ObjectModel;

namespace DevWinUIGallery.ViewModels;

public partial class ExtensionViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Animal> Items { get; set; } = new ObservableCollection<Animal>(Enum.GetValues<Animal>());

    [ObservableProperty]
    public partial bool IsCharacterValid { get; set; }

    [ObservableProperty]
    public partial bool IsPhoneNumberValid { get; set; }

    [ObservableProperty]
    public partial bool IsEmailValid { get; set; }

    [ObservableProperty]
    public partial bool IsDecimalValid { get; set; }

    [ObservableProperty]
    public partial bool IsNumberValid { get; set; }
}
