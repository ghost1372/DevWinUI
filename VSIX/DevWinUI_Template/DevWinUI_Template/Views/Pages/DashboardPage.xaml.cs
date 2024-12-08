using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        DataContext = this;

        if (!WizardConfig.IsBlank)
        {
            WizardConfig.UseEditorConfigFile = true;
            WizardConfig.UseJsonSettings = true;
            WizardConfig.UseSolutionFolder = true;
        }

        Loaded += DashboardPage_Loaded;
    }

    private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (WizardConfig.IsBlank)
        {
            if (tgJsonSettings != null)
            {
                tgJsonSettings.IsOn = false;
            }
            if (tgSolutionFolder != null)
            {
                tgSolutionFolder.IsOn = false;
            }
        }

        if (IsPreviewVSIX())
        {
            CmbVersion.SelectedIndex = 1;
        }
    }
    private bool IsPreviewVSIX()
    {
        var asm = Assembly.GetExecutingAssembly();
        var asmDir = Path.GetDirectoryName(asm.Location);
        var manifestPath = Path.Combine(asmDir, "extension.vsixmanifest");
        bool isPreview = false;
        if (File.Exists(manifestPath))
        {
            var doc = new XmlDocument();
            doc.Load(manifestPath);
            var metaData = doc.DocumentElement.ChildNodes.Cast<XmlElement>().FirstOrDefault(x => x.Name == "Metadata");
            var identity = metaData.ChildNodes.Cast<XmlElement>().FirstOrDefault(x => x.Name == "Preview");
            var value = identity?.InnerText;
            if (!string.IsNullOrEmpty(value))
            {
                isPreview = value.ToLower().Equals("true");
            }
        }
        return isPreview;
    }
    private void cmbVersionMechanism_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbVersionMechanism = sender as ComboBox;
        WizardConfig.UsePreReleaseVersion = cmbVersionMechanism.SelectedIndex != 0;

        if (LibrariesPage.Instance != null)
        {
            LibrariesPage.Instance.CreateBoxes();
        }
    }

    private void cmbNetVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        WizardConfig.DotNetVersion = (cmbNetVersion.SelectedItem as ComboBoxItem).Tag.ToString();

        if (LibrariesPage.Instance != null)
        {
            LibrariesPage.Instance.CreateBoxes();
        }
    }

    private void cmbTargetFrameworkVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        WizardConfig.TargetFrameworkVersion = (cmbTargetFrameworkVersion.SelectedItem as ComboBoxItem).Tag.ToString();
    }

    private void tgUnPackaged_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.IsUnPackagedMode = tgUnPackaged.IsOn;
    }

    private void tgNullable_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.Nullable = tgNullable.IsOn ? tgNullable.OnContent?.ToString() : tgNullable.OffContent?.ToString();
    }

    private void tgJsonSettings_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseJsonSettings = tgJsonSettings.IsOn;
    }

    private void tgSolutionFolder_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseSolutionFolder = tgSolutionFolder.IsOn;
    }

    private void txtSolutionFolderName_TextChanged(object sender, TextChangedEventArgs e)
    {
        var txt = sender as TextBox;
        if (!string.IsNullOrEmpty(txt.Text))
        {
            WizardConfig.SolutionFolderName = txt.Text;
        }
        else
        {
            WizardConfig.SolutionFolderName = WizardConfig.SolutionFolderNameDefault;
        }
    }
}
