namespace DevWinUI;
public static partial class ThemeServiceAttach
{
    public static IThemeService GetThemeService(DependencyObject obj)
    {
        return (IThemeService)obj.GetValue(ThemeServiceProperty);
    }

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

