namespace DevWinUI;
public partial class ThemeService
{
    public void OnThemeComboBoxSelectionChanged(object sender)
    {
        var cmb = (ComboBox)sender;
        var selectedTheme = (cmb?.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        if (selectedTheme != null)
        {
            ApplyThemeOrBackdrop<ElementTheme>(selectedTheme);
        }
    }

    public void SetThemeComboBoxDefaultItem(ComboBox themeComboBox)
    {
        var currentTheme = ElementTheme.ToString();
        var item = themeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme);
        if (item != null && (ComboBoxItem)themeComboBox.SelectedItem != item)
        {
            themeComboBox.SelectedItem = item;
        }
    }

    public void OnBackdropComboBoxSelectionChanged(object sender)
    {
        var cmb = (ComboBox)sender;
        var selectedBackdrop = (cmb?.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        if (selectedBackdrop != null)
        {
            selectedBackdrop = FixAcrylicBackdropTag(selectedBackdrop);
            ApplyThemeOrBackdrop<BackdropType>(selectedBackdrop);
        }
    }

    public void SetBackdropComboBoxDefaultItem(ComboBox backdropComboBox)
    {
        var currentBackdrop = FixAcrylicBackdropTag(BackdropType.ToString());

        var item = backdropComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(c => c?.Tag?.ToString() == currentBackdrop);
        if (item != null && (ComboBoxItem)backdropComboBox.SelectedItem != item)
        {
            backdropComboBox.SelectedItem = item;
        }
    }

    public void OnThemeRadioButtonChecked(object sender)
    {
        var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
        if (selectedTheme != null)
        {
            ApplyThemeOrBackdrop<ElementTheme>(selectedTheme);
        }
    }

    public void SetThemeRadioButtonDefaultItem(Panel ThemePanel)
    {
        var currentTheme = ElementTheme.ToString();
        var items = ThemePanel.Children.Cast<RadioButton>();
        if (items != null)
        {
            var selectedItem = items.FirstOrDefault(c => c?.Tag?.ToString() == currentTheme);
            if (selectedItem != null)
            {
                selectedItem.IsChecked = true;
            }
        }
    }

    public void OnBackdropRadioButtonChecked(object sender)
    {
        var selectedBackdrop = ((RadioButton)sender)?.Tag?.ToString();

        if (selectedBackdrop != null)
        {
            selectedBackdrop = FixAcrylicBackdropTag(selectedBackdrop);
            ApplyThemeOrBackdrop<BackdropType>(selectedBackdrop);
        }
    }

    public void SetBackdropRadioButtonDefaultItem(Panel BackdropPanel)
    {
        var currentBackdrop = BackdropType.ToString();
        var items = BackdropPanel.Children.Cast<RadioButton>();
        if (items != null)
        {
            currentBackdrop = FixAcrylicBackdropTag(currentBackdrop);
            var selectedItem = items.FirstOrDefault(c => c?.Tag?.ToString() == currentBackdrop);
            if (selectedItem != null)
            {
                selectedItem.IsChecked = true;
            }
        }
    }

    private async void ApplyThemeOrBackdrop<TEnum>(string text) where TEnum : struct
    {
        if (Enum.TryParse(text, out TEnum result) && Enum.IsDefined(typeof(TEnum), result))
        {
            if (result is BackdropType backdrop)
            {
                await SetBackdropTypeAsync(backdrop);
            }
            else if (result is ElementTheme theme)
            {
                await SetElementThemeAsync(theme);
            }
        }
    }

    private string FixAcrylicBackdropTag(string backdropTag)
    {
        if (string.IsNullOrEmpty(backdropTag))
            return backdropTag;

        if (backdropTag.Equals("DesktopAcrylic") || backdropTag.Equals("AcrylicBase"))
        {
            backdropTag = "Acrylic";
        }

        return backdropTag;
    }
}
