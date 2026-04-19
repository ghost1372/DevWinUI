namespace DevWinUI;
public static partial class ThemeServiceAttach
{
    /// <summary>
    /// Retrieves the theme service associated with a specified ComboBox.
    /// </summary>
    /// <param name="obj">The dependency object from which the theme service is being retrieved.</param>
    /// <returns>Returns the theme service linked to the provided dependency object.</returns>
    public static IThemeService GetThemeService(DependencyObject obj)
    {
        return (IThemeService)obj.GetValue(ThemeServiceProperty);
    }

    /// <summary>
    /// Sets a theme service for a specified ComboBox.
    /// </summary>
    /// <param name="obj">The dependency object that will receive the theme service.</param>
    /// <param name="value">The theme service instance to be associated with the dependency object.</param>
    public static void SetThemeService(DependencyObject obj, IThemeService value)
    {
        obj.SetValue(ThemeServiceProperty, value);
    }

    public static readonly DependencyProperty ThemeServiceProperty =
        DependencyProperty.RegisterAttached("ThemeService", typeof(IThemeService), typeof(ThemeServiceAttach),
        new PropertyMetadata(null, OnThemeServiceChanged));

    private static void OnThemeServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is IThemeService themeService)
        {
            if (d is ComboBox comboBox)
            {
                // Optional: Handle the Loaded event to set the default again when the ComboBox is loaded
                comboBox.Loaded += (sender, args) =>
                {
                    themeService.SetThemeComboBoxDefaultItem(comboBox);
                    themeService.SetBackdropComboBoxDefaultItem(comboBox);

                    void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
                    {
                        themeService.OnThemeComboBoxSelectionChanged(sender);
                        themeService.OnBackdropComboBoxSelectionChanged(sender);
                    }

                    comboBox.SelectionChanged -= OnComboBoxSelectionChanged;
                    comboBox.SelectionChanged += OnComboBoxSelectionChanged;
                };
            }
            else if (d is Panel themePanel)
            {
                // Optional: Handle the Loaded event to set the default again when the ComboBox is loaded
                themePanel.Loaded += (sender, args) =>
                {
                    themeService.SetThemeRadioButtonDefaultItem(themePanel);
                    themeService.SetBackdropRadioButtonDefaultItem(themePanel);

                    void OnRadioButtonChecked(object sender, RoutedEventArgs e)
                    {
                        themeService.OnThemeRadioButtonChecked(sender);
                        themeService.OnBackdropRadioButtonChecked(sender);
                    }
                    var items = themePanel.Children.Cast<RadioButton>();
                    foreach (var item in items)
                    {
                        item.Checked -= OnRadioButtonChecked;
                        item.Checked += OnRadioButtonChecked;
                    }
                };
            }
        }
    }
}

