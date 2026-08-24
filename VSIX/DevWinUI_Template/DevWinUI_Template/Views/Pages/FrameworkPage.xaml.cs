using DevWinUI_Template.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevWinUI_Template;

public partial class FrameworkPage : Page
{
    public FrameworkPage()
    {
        InitializeComponent();

        DataContext = WizardConfig.Current;

        RadioLV.SelectedValue = WizardConfig.Current.DotNetVersion;

        Loaded += FrameworkPage_Loaded;
        PreviewMouseWheel += FrameworkPage_PreviewMouseWheel;
    }

    private void FrameworkPage_Loaded(object sender, RoutedEventArgs e)
    {
        Helper.UpdateCornerRadius(RadioLV);
        Helper.UpdateActiveCornerRadius(RadioLV);
    }

    private void RadioLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        Helper.UpdateActiveCornerRadius(RadioLV);
    }

    private void FrameworkPage_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!(e.OriginalSource is DependencyObject source)) return;

        var parent = source as DependencyObject;
        while (parent != null && !(parent is ScrollViewer))
        {
            parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
        }

        if (parent is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
