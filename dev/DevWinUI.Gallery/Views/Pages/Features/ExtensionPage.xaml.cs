using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public sealed partial class ExtensionPage : Page
{
    public ObservableCollection<Animal> Items
    {
        get { return (ObservableCollection<Animal>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<Animal>), typeof(ExtensionPage), new PropertyMetadata(new ObservableCollection<Animal>(Enum.GetValues<Animal>())));

    public ExtensionPage()
    {
        this.InitializeComponent();
    }
}
