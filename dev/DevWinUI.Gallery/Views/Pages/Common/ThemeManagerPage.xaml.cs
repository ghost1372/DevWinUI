using System.Threading.Tasks;
using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class ThemeManagerPage : Page
{
    public BaseViewModel ViewModel { get;}
    public ThemeManagerPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        this.InitializeComponent();
    }

    private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        var item = sender as RadioButton;
        cmbTheme.SelectedItem = GeneralHelper.GetEnum<ElementTheme>(item.Tag?.ToString());
    }
    private void OnBackdropRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        var item = sender as RadioButton;
        cmbBackdrop.SelectedItem = GeneralHelper.GetEnum<BackdropType>(item.Tag?.ToString());
    }
    private async void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var theme = cmbTheme.SelectedItem.As<ElementTheme>();
        await App.Current.ThemeService.SetElementThemeAsync(theme);
        foreach (RadioButton item in themePanel.Children)
        {
            if (item.Tag.Equals(theme.ToString()))
            {
                item.IsChecked = true;
                return;
            }
        }
    }
    private async void cmbBackdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var backdrop = cmbBackdrop.SelectedItem.As<BackdropType>();
        await App.Current.ThemeService.SetBackdropTypeAsync(backdrop);
        foreach (RadioButton item in backdropPanel.Children)
        {
            if (item.Tag.Equals(backdrop.ToString()))
            {
                item.IsChecked = true;
                return;
            }
        }
    }
}
