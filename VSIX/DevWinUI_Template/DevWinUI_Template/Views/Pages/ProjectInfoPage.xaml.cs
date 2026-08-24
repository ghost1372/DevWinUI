using System.Windows;
using System.Windows.Controls;

namespace DevWinUI_Template;

public partial class ProjectInfoPage : Page
{
    public ProjectInfoPage()
    {
        InitializeComponent();

        DataContext = WizardConfig.Current;
    }
}
