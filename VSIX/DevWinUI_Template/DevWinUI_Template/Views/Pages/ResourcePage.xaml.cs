using System.Windows;
using System.Windows.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public partial class ResourcePage : Page
{
    public ResourcePage()
    {
        InitializeComponent();
    }

    private void tgDicColor_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.UseColorsDic = tgDicColor.IsOn;
    }

    private void tgDicStyle_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.UseStylesDic = tgDicStyle.IsOn;
    }

    private void tgDicFont_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.UseFontsDic = tgDicFont.IsOn;
    }

    private void tgDicConverter_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.UseConvertersDic = tgDicConverter.IsOn;
    }
}
