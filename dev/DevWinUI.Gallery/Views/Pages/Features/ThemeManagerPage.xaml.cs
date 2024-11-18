namespace DevWinUIGallery.Views;

public sealed partial class ThemeManagerPage : Page
{
    public ThemeManagerPage()
    {
        this.InitializeComponent();
    }

    private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        App.Current.GetThemeService.OnThemeRadioButtonChecked(sender);
    }
    private void OnBackdropRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        App.Current.GetThemeService.OnBackdropRadioButtonChecked(sender);
    }
    private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.Current.GetThemeService.OnThemeComboBoxSelectionChanged(sender);
    }
    private void cmbBackdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.Current.GetThemeService.OnBackdropComboBoxSelectionChanged(sender);
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        App.Current.GetThemeService.SetThemeRadioButtonDefaultItem(themePanel);
        App.Current.GetThemeService.SetBackdropRadioButtonDefaultItem(backdropPanel);
        App.Current.GetThemeService.SetThemeComboBoxDefaultItem(cmbTheme);
        App.Current.GetThemeService.SetBackdropComboBoxDefaultItem(cmbBackdrop);
    }
}
