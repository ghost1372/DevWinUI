using DevWinUI_Template.Models;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DevWinUI_Template;

public class WinUIAppMVVMNavigationWizard : IWizard
{
    private _DTE _dte;

    private string vsTemplateFilePath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}\ProjectTemplates\CSharp\1033\{Template}\{Template}.vstemplate
    private string projectTemplatesFolderPath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}\ProjectTemplates\CSharp\1033\{Template}
    private string vsixRootFolderPath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}

    private string projectName; // App
    private string safeProjectName; // App
    private string specifiedSolutionName; // App
    private string solutionDirectory; // E:\\source\\App
    private string destinationDirectory;// E:\source\App\App

    private Project _project;
    public void BeforeOpeningFile(ProjectItem projectItem)
    {
    }

    public void ProjectFinishedGenerating(Project project)
    {
        _project = project;

        if (WizardConfig.Current.ShowStartupToolWindow)
        {
            WizardHelper.OpenStartupToolWindow();
        }
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
    }

    public async void RunFinished()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var _solution = (Solution2)_dte.Solution;

        WizardHelper.AddGithubActionFile(_project, _dte, vsixRootFolderPath, solutionDirectory);
        WizardHelper.AddXamlStylerConfigFile(_dte, vsixRootFolderPath, solutionDirectory);

        WizardHelper.AddSolutionFolder(_solution);

        var appXaml = _solution.FindProjectItem("App.xaml");
        var appXamlCS = _solution.FindProjectItem("App.xaml.cs");
        var settingsPageXaml = _solution.FindProjectItem("SettingsPage.xaml");
        var generalSettingsPageXaml = _solution.FindProjectItem("GeneralSettingPage.xaml");

        VSDocumentHelper.FormatDocument(_dte, appXaml);
        VSDocumentHelper.FormatDocument(_dte, appXamlCS);
        VSDocumentHelper.FormatDocument(_dte, settingsPageXaml);
        VSDocumentHelper.FormatDocument(_dte, generalSettingsPageXaml);

        foreach (Document doc in _dte.Documents)
        {
            doc.Close();
        }
    }

    public async void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        _dte = automationObject as _DTE;

        projectName = replacementsDictionary["$projectname$"];
        safeProjectName = replacementsDictionary["$safeprojectname$"];
        specifiedSolutionName = replacementsDictionary["$specifiedsolutionname$"];
        solutionDirectory = replacementsDictionary["$solutiondirectory$"];
        destinationDirectory = replacementsDictionary["$destinationdirectory$"];

        var createdApplication = false;
        if (Application.Current == null)
        {
            _ = new Application
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            createdApplication = true;
        }

        var wizardWindow = new MainWindow();
        var accepted = wizardWindow.ShowDialog();

        if (createdApplication)
        {
            Application.Current?.Shutdown();
        }

        if (accepted.HasValue && accepted.Value)
        {
            var vsix = await WizardHelper.GetVSIXPathAsync(_dte, "WinUIApp-MVVM-NavigationView");
            vsTemplateFilePath = vsix.VSTemplatePath;
            projectTemplatesFolderPath = vsix.ProjectTemplatesFolder;
            vsixRootFolderPath = vsix.VSIXRootFolder;

            WizardHelper.AddEditorConfigFile(_dte, vsixRootFolderPath, solutionDirectory);

            ConfigureTemplateValues(replacementsDictionary);
        }
        else
        {
            wizardWindow.Close();
            throw new WizardCancelledException();
        }
    }
    private void ConfigureTemplateValues(Dictionary<string, string> replacementsDictionary)
    {
        var serilogOptions = new SerilogOption();
        serilogOptions.ConfigSerilog(replacementsDictionary);

        new GlobalUsingOption(
            replacementsDictionary,
            safeProjectName,
            WizardConfig.Current.UseJsonSettings,
            WizardConfig.Current.UseFileLogger || WizardConfig.Current.UseDebugLogger);

        if (WizardConfig.Current.UseJsonSettings)
        {
            WizardConfig.Current.NuGetPackages.Add(new NugetPackageModel("nucs.JsonSettings"));
            WizardConfig.Current.NuGetPackages.Add(new NugetPackageModel("nucs.JsonSettings.Autosave"));
            WizardConfig.Current.NuGetPackages.Add(new NugetPackageModel("nucs.JsonSettings.NotifyChanges"));
        }

        new ColorsDicOption().ConfigColorsDic(replacementsDictionary);

        var configCodes = new ConfigCodes();
        configCodes.ConfigAllMVVM(safeProjectName);
        configCodes.ConfigGeneral();

        ApplyConfigCodeReplacements(replacementsDictionary, configCodes);
        ApplyLoggingReplacements(replacementsDictionary);
        AddReplacementsDictionary(replacementsDictionary);
    }

    private void ApplyConfigCodeReplacements(Dictionary<string, string> replacementsDictionary, ConfigCodes configCodes)
    {
        var configs = configCodes.GetConfigJson();
        var services = configCodes.GetServices();
        var settingsCards = configCodes.GetSettingsPageOptions();
        var generalSettingsCards = configCodes.GetGeneralSettingsPageOptions();

        replacementsDictionary["$ConfigDefaultPages$"] = configCodes.ConfigJsonDic.Count > 0
            ? "\n" + FormatIndentedMultiline(configs)
            : string.Empty;

        replacementsDictionary["$Services$"] = configCodes.ServiceDic.Count > 0
            ? Environment.NewLine + services
            : string.Empty;

        replacementsDictionary["$SettingsCards$"] = settingsCards;
        replacementsDictionary["$GeneralSettingsCards$"] = !string.IsNullOrEmpty(generalSettingsCards)
            ? generalSettingsCards
            : string.Empty;
    }

    private static string FormatIndentedMultiline(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = "                " + lines[i].Trim();
        }

        return string.Join("\n", lines);
    }

    private void ApplyLoggingReplacements(Dictionary<string, string> replacementsDictionary)
    {
        var generalSettingsCards = replacementsDictionary.TryGetValue("$GeneralSettingsCards$", out var configuredCards)
            ? configuredCards
            : string.Empty;

        replacementsDictionary.AddIfNotExists("$GeneralSettingsCards$", generalSettingsCards);

        if (WizardConfig.Current.UseDebugLogger || WizardConfig.Current.UseFileLogger)
        {
            if (WizardConfig.Current.UseJsonSettings &&
                WizardConfig.Current.UseDeveloperModeSetting &&
                WizardConfig.Current.UseSettingsPage &&
                WizardConfig.Current.UseGeneralSettingPage)
            {
                replacementsDictionary.AddIfNotExists("$GoToLogPathEvent$", Environment.NewLine + Environment.NewLine + PredefinedCodes.GoToLogPathEvent);
                replacementsDictionary.AddIfNotExists("$DeveloperModeConfig$", Environment.NewLine + "public bool UseDeveloperMode { get; set; }");
            }
            else
            {
                replacementsDictionary.AddIfNotExists("$GoToLogPathEvent$", "");
                replacementsDictionary.AddIfNotExists("$DeveloperModeConfig$", "");
            }
        }
        else
        {
            replacementsDictionary.AddIfNotExists("$GoToLogPathEvent$", "");
            replacementsDictionary.AddIfNotExists("$DeveloperModeConfig$", "");
        }
    }

    private void AddReplacementsDictionary(Dictionary<string, string> replacementsDictionary)
    {
        // Add Base Library Versions
        replacementsDictionary.Add("$DotNetVersion$", WizardConfig.Current.DotNetVersion.ToString());
        replacementsDictionary.Add("$TargetFrameworkVersion$", WizardConfig.Current.TargetFrameworkVersion.ToString());

        replacementsDictionary.Add("$Nullable$", WizardConfig.Current.EnableNullableReferenceTypes ? "enable" : "disable");
        replacementsDictionary.Add("$RegistryWriteVirtualizationState$", WizardConfig.Current.RegistryVirtualizationDisabled ? "disabled" : "enabled");
        replacementsDictionary.Add("$FileSystemWriteVirtualizationState$", WizardConfig.Current.FileVirtualizationDisabled ? "disabled" : "enabled");

        replacementsDictionary.Add("$AddJsonSettings$", WizardConfig.Current.UseJsonSettings.ToString());
        replacementsDictionary.Add("$AddEditorConfig$", WizardConfig.Current.UseEditorConfigFile.ToString());
        replacementsDictionary.Add("$AddSolutionFolder$", WizardConfig.Current.UseSolutionFolder.ToString());
        replacementsDictionary.Add("$AddHomeLandingPage$", WizardConfig.Current.UseHomeLandingPage.ToString());
        replacementsDictionary.Add("$AddSettingsPage$", WizardConfig.Current.UseSettingsPage.ToString());
        replacementsDictionary.Add("$AddGeneralSettingPage$", WizardConfig.Current.UseGeneralSettingPage.ToString());
        replacementsDictionary.Add("$AddThemeSettingPage$", WizardConfig.Current.UseThemeSettingPage.ToString());
        replacementsDictionary.Add("$AddAppUpdatePage$", WizardConfig.Current.UseAppUpdatePage.ToString());
        replacementsDictionary.Add("$AddAboutPage$", WizardConfig.Current.UseAboutPage.ToString());

        // JSON does not allow comments; when the home page is disabled, omit the item instead of emitting a commented-out object.
        replacementsDictionary.Add("$HomeLandingMenuItem$", WizardConfig.Current.UseHomeLandingPage
        ? "{" + Environment.NewLine +
          "\t\t\t\t\t\"UniqueId\": \"$safeprojectname$.Views.HomeLandingPage\"," + Environment.NewLine +
          "\t\t\t\t\t\"Title\": \"$safeprojectname$\"," + Environment.NewLine +
          "\t\t\t\t\t\"Subtitle\": \"$safeprojectname$\"," + Environment.NewLine +
          "\t\t\t\t\t\"ImagePath\": \"ms-appx:///Assets/AppIcon.png\"," + Environment.NewLine +
          "\t\t\t\t\t\"HideItem\": true" + Environment.NewLine +
          "\t\t\t\t}"
        : string.Empty);

        var hasBreadcrumbPages = WizardConfig.Current.UseSettingsPage && (
            WizardConfig.Current.UseGeneralSettingPage ||
            WizardConfig.Current.UseThemeSettingPage ||
            WizardConfig.Current.UseAppUpdatePage ||
            WizardConfig.Current.UseAboutPage);
        replacementsDictionary.Add("$BreadcrumbBarConfig$", hasBreadcrumbPages
            ? Environment.NewLine + "                .ConfigureBreadcrumbBar(BreadCrumbNav, BreadcrumbPageMappings.PageDictionary)"
            : string.Empty);

        #region IsUnPackaged
        if (WizardConfig.Current.IsUnPackaged)
        {
            replacementsDictionary.Add("$WindowsPackageType$", "None");
            replacementsDictionary.Add("$UAP5$", "");
            replacementsDictionary.Add("$StartupTask$", "");
        }
        else
        {
            replacementsDictionary.Add("$WindowsPackageType$", "MSIX");

            if (WizardConfig.Current.UseGeneralSettingPage && WizardConfig.Current.UseStartupSetting)
            {
                replacementsDictionary.Add("$UAP5$", Environment.NewLine + "  xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\"");

                var taskContent = PredefinedCodes.StartupTask.Replace("$safeprojectname$", safeProjectName);
                replacementsDictionary.Add("$StartupTask$", Environment.NewLine + taskContent);
            }
            else
            {
                replacementsDictionary.Add("$UAP5$", "");
                replacementsDictionary.Add("$StartupTask$", "");
            }
        }
        #endregion

        #region Json Settings
        if (WizardConfig.Current.UseJsonSettings)
        {
            replacementsDictionary.Add("$AppUpdateMVVMGetDateTime$", Environment.NewLine + """LastUpdateCheck = Settings.LastUpdateCheck;""");
            replacementsDictionary.Add("$AppUpdateMVVMSetDateTime$", Environment.NewLine + """Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();""");

            replacementsDictionary.Add("$AppConfigFilePath$", Environment.NewLine + """public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");""");

            if (WizardConfig.Current.UseAppUpdatePage && WizardConfig.Current.UseSettingsPage)
            {
                replacementsDictionary.Add("$AppUpdateConfig$", Environment.NewLine + """public string LastUpdateCheck { get; set; }""");
            }
            else
            {
                replacementsDictionary.Add("$AppUpdateConfig$", "");
            }
        }
        else
        {
            replacementsDictionary.Add("$AppUpdateMVVMGetDateTime$", "");
            replacementsDictionary.Add("$AppUpdateMVVMSetDateTime$", "");
            replacementsDictionary.Add("$AppConfigFilePath$", "");
            replacementsDictionary.Add("$AppUpdateConfig$", "");
        }
        #endregion

        StringBuilder stringBuilder = new StringBuilder();
        if (WizardConfig.Current.NuGetPackages.Count > 0)
        {
            foreach (var item in WizardConfig.Current.NuGetPackages)
            {
                // indent each PackageReference so it aligns in the .csproj file
                stringBuilder.Append("    ");
                stringBuilder.AppendLine($"<PackageReference Include=\"{item.PackageName}\" Version=\"*\" />");
            }

            // ensure there's a newline before the first package so it doesn't get concatenated with previous content
            replacementsDictionary.Add("$ExtraNuGetPackages$", Environment.NewLine + stringBuilder.ToString().TrimEnd());
        }
        else
        {
            replacementsDictionary.Add("$ExtraNuGetPackages$", string.Empty);
        }
    }

    public bool ShouldAddProjectItem(string filePath)
    {
        if (!WizardConfig.Current.UseHomeLandingPage &&
            (filePath.Contains("HomeLanding")))
        {
            return false;
        }
        else if (!WizardConfig.Current.UseSettingsPage &&
            (filePath.Contains("SettingsPage.xaml") ||
            filePath.Contains("AboutUsSettingPage") ||
            filePath.Contains("ThemeSettingPage") ||
            filePath.Contains("AboutUsSettingViewModel") ||
            filePath.Contains("GeneralSettingPage") ||
            filePath.Contains("GeneralSettingViewModel") ||
            filePath.Contains("AppUpdateSettingPage") ||
            filePath.Contains("AppUpdateSettingViewModel") ||
            filePath.Contains("Backdrop.png") ||
            filePath.Contains("Color.png") ||
            filePath.Contains("External.png") ||
            filePath.Contains("Info.png") ||
            filePath.Contains("General.png") ||
            filePath.Contains("Theme.png") ||
            filePath.Contains("DevMode.png") ||
            filePath.Contains("Update.png")))
        {
            return false;
        }
        else if (WizardConfig.Current.UseSettingsPage &&
            !WizardConfig.Current.UseAboutPage &&
            (filePath.Contains("AboutUsSettingPage") ||
            filePath.Contains("AboutUsSettingViewModel") ||
            filePath.Contains("Info.png")))
        {
            return false;
        }
        else if (WizardConfig.Current.UseSettingsPage &&
            !WizardConfig.Current.UseThemeSettingPage &&
            (filePath.Contains("ThemeSettingPage") ||
            filePath.Contains("Backdrop.png") ||
            filePath.Contains("Color.png") ||
            filePath.Contains("External.png") ||
            filePath.Contains("Theme.png")))
        {
            return false;
        }
        else if (WizardConfig.Current.UseSettingsPage &&
            !WizardConfig.Current.UseGeneralSettingPage &&
            (filePath.Contains("GeneralSettingPage") ||
            filePath.Contains("GeneralSettingViewModel") ||
            filePath.Contains("General.png") ||
            filePath.Contains("Startup.png")))
        {
            return false;
        }
        else if (WizardConfig.Current.UseSettingsPage &&
            !WizardConfig.Current.UseAppUpdatePage &&
            (filePath.Contains("AppUpdateSettingPage") ||
            filePath.Contains("AppUpdateSettingViewModel") ||
            filePath.Contains("Update.png")))
        {
            return false;
        }
        else if (!WizardConfig.Current.UseJsonSettings &&
            (filePath.Contains("AppConfig") ||
            filePath.Contains("AppHelper")))
        {
            return false;
        }
        else if (!WizardConfig.Current.UseDeveloperModeSetting && filePath.Contains("DevMode.png"))
        {
            return false;
        }
        else if (!WizardConfig.Current.UseDebugLogger &&
            !WizardConfig.Current.UseFileLogger &&
            filePath.Contains("LoggerSetup"))
        {
            return false;
        }
        else if (!WizardConfig.Current.UseStartupSetting && filePath.Contains("Startup.png"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
