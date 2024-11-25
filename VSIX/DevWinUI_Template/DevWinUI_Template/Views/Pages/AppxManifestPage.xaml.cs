using System;
using System.Windows;
using System.Windows.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public partial class AppxManifestPage : Page
{
    public AppxManifestPage()
    {
        InitializeComponent();
    }

    private void Toggled(object sender, RoutedEventArgs e)
    {
        var optionUC = sender as OptionUCNoExpander;
        AddOrRemoveElement(optionUC);
    }

    private void AddOrRemoveElement(OptionUCNoExpander optionUC)
    {
        try
        {
            string keyValue = optionUC.Tag.ToString();
            if (optionUC.IsOn)
            {
                WizardConfig.UnvirtualizedResources.TryGetValue(keyValue, out var valueExist);
                if (!string.IsNullOrEmpty(valueExist))
                {
                    WizardConfig.UnvirtualizedResources.Remove(keyValue);
                }
                WizardConfig.MinimumTargetPlatform = WizardConfig.MinimumTargetPlatformDefault;
            }
            else
            {
                WizardConfig.UnvirtualizedResources.AddIfNotExists(keyValue, $"    <{keyValue}>disabled</{keyValue}>");
                WizardConfig.MinimumTargetPlatform = "18362";
            }
        }
        catch (Exception)
        {

        }
    }
}
