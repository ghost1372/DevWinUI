using System.Windows.Controls;
using DevWinUI_Template.WizardUI;
using EnvDTE80;

namespace DevWinUI_Template;

public partial class FilePage : Page
{
    public FilePage()
    {
        InitializeComponent();
        Loaded += FilePage_Loaded;
    }

    private void FilePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (WizardConfig.IsBlank)
        {
            if (tgEditorConfig != null)
            {
                tgEditorConfig.IsOn = false;
            }
        }
    }

    private void tgEditorConfig_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseEditorConfigFile = tgEditorConfig.IsOn;
    }
    private void tgGithubWorkflow_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseGithubWorkflowFile = tgGithubWorkflow.IsOn;
    }

    private void tgXamlStyler_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseXamlStylerFile = tgXamlStyler.IsOn;
    }
}
