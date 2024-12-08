namespace DevWinUI_Template.Views.Startup;

using System.Diagnostics;
using System.Windows.Controls;

public partial class StartupToolWindowControl : UserControl
{
    public StartupToolWindowControl()
    {
        this.InitializeComponent();
    }

    private void ImageIcon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Process.Start(MSStoreHyperLink.NavigateUri);
    }
}
