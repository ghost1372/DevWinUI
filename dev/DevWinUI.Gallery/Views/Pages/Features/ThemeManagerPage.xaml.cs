namespace DevWinUIGallery.Views;

public sealed partial class ThemeManagerPage : Page
{
    public ThemeManagerPage()
    {
        this.InitializeComponent();
    }

    private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        App.Current.ThemeService.OnThemeRadioButtonChecked(sender);
    }
    private void OnBackdropRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        App.Current.ThemeService.OnBackdropRadioButtonChecked(sender);
    }
    private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.Current.ThemeService.OnThemeComboBoxSelectionChanged(sender);
    }
    private void cmbBackdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.Current.ThemeService.OnBackdropComboBoxSelectionChanged(sender);
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        App.Current.ThemeService.SetThemeRadioButtonDefaultItem(themePanel);
        App.Current.ThemeService.SetBackdropRadioButtonDefaultItem(backdropPanel);
        App.Current.ThemeService.SetThemeComboBoxDefaultItem(cmbTheme);
        App.Current.ThemeService.SetBackdropComboBoxDefaultItem(cmbBackdrop);
    }
}
