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
    private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var theme = cmbTheme.SelectedItem.As<ElementTheme>();
        App.Current.ThemeService.SetElementTheme(theme);
        foreach (RadioButton item in themePanel.Children)
        {
            if (item.Tag.Equals(theme.ToString()))
            {
                item.IsChecked = true;
                return;
            }
        }
    }
    private void cmbBackdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var backdrop = cmbBackdrop.SelectedItem.As<BackdropType>();
        App.Current.ThemeService.SetBackdropType(backdrop);
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
