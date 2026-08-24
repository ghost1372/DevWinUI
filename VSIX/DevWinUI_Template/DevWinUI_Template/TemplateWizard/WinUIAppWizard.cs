using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;

namespace DevWinUI_Template;

public class WinUIAppWizard : IWizard
{
    private _DTE _dte;
    private string safeProjectName; // App
    private string solutionDirectory; // E:\\source\\App
    private string vsixRootFolderPath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}

    public void BeforeOpeningFile(ProjectItem projectItem)
    {
    }

    public void ProjectFinishedGenerating(Project project)
    {
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
    }

    public void RunFinished()
    {

    }

    public async void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        _dte = automationObject as _DTE;

        var vsix = await WizardHelper.GetVSIXPathAsync(_dte, "WinUIApp");

        vsixRootFolderPath = vsix.VSIXRootFolder;
        safeProjectName = replacementsDictionary["$safeprojectname$"];
        solutionDirectory = replacementsDictionary["$solutiondirectory$"];

        WizardHelper.AddEditorConfigFile(_dte, vsixRootFolderPath, solutionDirectory);

        new GlobalUsingOption(replacementsDictionary, safeProjectName);

        AddReplacementsDictionary(replacementsDictionary);
    }
    private void AddReplacementsDictionary(Dictionary<string, string> replacementsDictionary)
    {
        replacementsDictionary.Add("$DotNetVersion$", WizardConfig.Current.DotNetVersion.ToString());
        replacementsDictionary.Add("$TargetFrameworkVersion$", WizardConfig.Current.TargetFrameworkVersion.ToString());

        if (WizardConfig.Current.IsUnPackaged)
        {
            replacementsDictionary.Add("$WindowsPackageType$", "None");
        }
        else
        {
            replacementsDictionary.Add("$WindowsPackageType$", "MSIX");
        }

        if (WizardConfig.Current.UseJsonSettings)
        {
            replacementsDictionary.Add("$AppConfigFilePath$", Environment.NewLine + """public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");""");
        }
        else
        {
            replacementsDictionary.Add("$AppConfigFilePath$", "");
        }
    }

    public bool ShouldAddProjectItem(string filePath)
    {
        return true;
    }
}
