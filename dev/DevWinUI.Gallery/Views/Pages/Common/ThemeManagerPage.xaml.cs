namespace DevWinUIGallery.Views;

public sealed partial class ThemeManagerPage : Page
{
    public ThemeManagerPage()
    {
        this.InitializeComponent();
    }

    private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton { Tag: string tag })
            return;

        cmbTheme.Value = GeneralHelper.GetEnum<ElementTheme>(tag);
    }
    private void OnBackdropRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton { Tag: string tag })
            return;

        cmbBackdrop.Value = GeneralHelper.GetEnum<BackdropType>(tag);
    }
    private async void cmbTheme_ValueChanged(ChoiceOption sender, object value)
    {
        var theme = (ElementTheme)value;
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
    private async void cmbBackdrop_ValueChanged(ChoiceOption sender, object value)
    {
        var backdrop = (BackdropType)value;
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
