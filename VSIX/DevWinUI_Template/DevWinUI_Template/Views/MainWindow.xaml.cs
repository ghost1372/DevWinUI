using System.Windows;
using System;
using Wpf.Ui.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new System.Uri("pack://application:,,,/Wpf.Ui;component/Resources/Theme/Light.xaml", UriKind.RelativeOrAbsolute)
        });
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new System.Uri("/DevWinUI_Template;component/Theme/Generic.xaml", UriKind.RelativeOrAbsolute)
        });
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new System.Uri("/DevWinUI_Template;component/Theme/TextBlockStyle.xaml", UriKind.RelativeOrAbsolute)
        });
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new System.Uri("/DevWinUI_Template;component/Theme/Controls/SettingsControl.xaml", UriKind.RelativeOrAbsolute)
        });
        InitializeComponent();
        Loaded += MainWindowWizard_Loaded;
    }

    private void MainWindowWizard_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
                this,                                    
                Wpf.Ui.Controls.WindowBackdropType.Mica, 
                true                                     
            );
        }
        catch (Exception)
        {
        }
        NviPage.IsEnabled = WizardConfig.HasPages;
        if (WizardConfig.IsBlank)
        {
            NviPage.IsEnabled = false;
        }
        RootNavigation.Navigate(typeof(DashboardPage));
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DialogResult.HasValue && DialogResult.Value)
        {
        }
        else
        {
            Cancel();
        }
    }

    private void btnCreate_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;

        Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Cancel();
    }

    private void Cancel()
    {
        Resources?.Clear();
        Application.Current?.Resources?.Clear();
        DialogResult = false;
    }
}
