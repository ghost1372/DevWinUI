using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;
public sealed partial class HatchPage : Page
{
    public ObservableCollection<HatchStyle> Items
    {
        get { return (ObservableCollection<HatchStyle>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<HatchStyle>), typeof(SwitchPresenterPage), new PropertyMetadata(new ObservableCollection<HatchStyle>(Enum.GetValues<HatchStyle>())));

    public HatchPage()
    {
        this.InitializeComponent();
    }

    private void HatchPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HatchSample.HatchStyle = GeneralHelper.GetEnum<HatchStyle>(HatchPicker.SelectedItem.ToString());
    }
}
