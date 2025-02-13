using System.Collections.Generic;
using DevWinUI_Template.WizardUI;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace DevWinUI_Template;

public class WinUIAppMVVMNavigationWizard : IWizard
{
    SharedWizard WizardImplementation;

    public void BeforeOpeningFile(ProjectItem projectItem)
    {
    }

    public void ProjectFinishedGenerating(Project project)
    {
        WizardImplementation.ProjectFinishedGenerating(project);
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
    }

    public void RunFinished()
    {
        WizardImplementation.RunFinished();
    }

    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
    {
        WizardImplementation = new SharedWizard();
        WizardImplementation.RunStarted(automationObject, replacementsDictionary, new TemplateConfig { HasPages = true, IsMVVM = true, HasNavigationView = true, TemplateType = TemplateType.WinUIApp_MVVM_NavigationView });
    }

    public bool ShouldAddProjectItem(string filePath)
    {
        if (!WizardImplementation.ShouldAddProjectItem())
        {
            return false;
        }

        if (!WizardConfig.UseHomeLandingPage &&
            (filePath.Contains("HomeLanding")))
        {
            return false;
        }
        else if (!WizardConfig.UseSettingsPage &&
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
            filePath.Contains("Tint.png") ||
            filePath.Contains("External.png") ||
            filePath.Contains("Info.png") ||
            filePath.Contains("General.png") ||
            filePath.Contains("Theme.png") ||
            filePath.Contains("DevMode.png") ||
            filePath.Contains("Update.png")))
        {
            return false;
        }
        else if (WizardConfig.UseSettingsPage &&
            !WizardConfig.UseAboutPage &&
            (filePath.Contains("AboutUsSettingPage") ||
            filePath.Contains("AboutUsSettingViewModel") ||
            filePath.Contains("Info.png")))
        {
            return false;
        }
        else if (WizardConfig.UseSettingsPage &&
            !WizardConfig.UseThemeSettingPage &&
            (filePath.Contains("ThemeSettingPage") ||
            filePath.Contains("Backdrop.png") ||
            filePath.Contains("Color.png") ||
            filePath.Contains("Tint.png") ||
            filePath.Contains("External.png") ||
            filePath.Contains("Theme.png")))
        {
            return false;
        }
        else if (WizardConfig.UseSettingsPage &&
            !WizardConfig.UseGeneralSettingPage &&
            (filePath.Contains("GeneralSettingPage") ||
            filePath.Contains("GeneralSettingViewModel") ||
            filePath.Contains("General.png") ||
            filePath.Contains("Startup.png")))
        {
            return false;
        }
        else if (WizardConfig.UseSettingsPage &&
            !WizardConfig.UseAppUpdatePage &&
            (filePath.Contains("AppUpdateSettingPage") ||
            filePath.Contains("AppUpdateSettingViewModel") ||
            filePath.Contains("Update.png")))
        {
            return false;
        }
        else if (!WizardConfig.UseJsonSettings &&
            (filePath.Contains("AppConfig") ||
            filePath.Contains("AppHelper")))
        {
            return false;
        }
        else if (!WizardConfig.UseHomeLandingPage &&
            !WizardConfig.UseColorsDic &&
            filePath.Contains("ThemeResources.xaml"))
        {
            return false;
        }
        else if (filePath.Contains("Resources") &&
            !filePath.Contains("ThemeResources"))
        {
            return false;
        }
        else if (!WizardConfig.UseDeveloperModeSetting && filePath.Contains("DevMode.png"))
        {
            return false;
        }
        else if (!WizardImplementation.UseDebugLogger &&
            !WizardImplementation.UseFileLogger &&
            filePath.Contains("LoggerSetup"))
        {
            return false;
        }
        else if (!WizardConfig.UseConvertersDic && filePath.Contains("Converters.xaml"))
        {
            return false;
        }
        else if (!WizardConfig.UseFontsDic && filePath.Contains("Fonts.xaml"))
        {
            return false;
        }
        else if (!WizardConfig.UseWindow11ContextMenu && filePath.Contains("Package-managed.WinContextMenu.appxmanifest"))
        {
            return false;
        }
        else if (WizardConfig.UseWindow11ContextMenu && filePath.Contains("Package-managed.appxmanifest"))
        {
            return false;
        }
        else if (!WizardConfig.UseStartupSetting && filePath.Contains("Startup.png"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
