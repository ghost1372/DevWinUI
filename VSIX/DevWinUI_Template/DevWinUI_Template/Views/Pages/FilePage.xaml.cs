using System.Windows.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template
{
    public partial class FilePage : Page
    {
        public FilePage()
        {
            InitializeComponent();
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
}
