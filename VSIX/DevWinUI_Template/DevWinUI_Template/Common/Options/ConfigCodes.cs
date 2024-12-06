using System;
using System.Collections.Generic;
using System.Text;

namespace DevWinUI_Template;

public class ConfigCodes
{
    public Dictionary<string, string> ConfigJsonDic = new();
    public Dictionary<string, string> ServiceDic = new();
    public Dictionary<string, string> SettingsPageOptionsDic = new();
    public Dictionary<string, string> GeneralSettingsPageOptionsDic = new();

    bool UseAboutPage;
    bool UseAppUpdatePage;
    bool UseGeneralSettingPage;
    bool UseHomeLandingPage;
    bool UseSettingsPage;
    bool UseThemeSettingPage;
    bool UseDeveloperModeSetting;
    bool UseJsonSetting;
    bool UseContextMenu;
    public ConfigCodes(bool UseAboutPage, bool UseAppUpdatePage, bool UseGeneralSettingPage, bool UseHomeLandingPage, bool UseSettingsPage, bool UseThemeSettingPage, bool UseDeveloperModeSetting, bool UseJsonSetting, bool useContextMenu)
    {
        this.UseAboutPage = UseAboutPage;
        this.UseAppUpdatePage = UseAppUpdatePage;
        this.UseGeneralSettingPage = UseGeneralSettingPage;
        this.UseHomeLandingPage = UseHomeLandingPage;
        this.UseSettingsPage = UseSettingsPage;
        this.UseThemeSettingPage = UseThemeSettingPage;
        this.UseDeveloperModeSetting = UseDeveloperModeSetting;
        this.UseJsonSetting = UseJsonSetting;
        UseContextMenu = useContextMenu;
    }

    public string GetConfigJson()
    {
        StringBuilder outputBuilder = new StringBuilder();
        foreach (var item in ConfigJsonDic.Values)
        {
            outputBuilder.AppendLine(item);
        }

        return outputBuilder.ToString().Trim();
    }

    public string GetServices()
    {
        StringBuilder outputBuilder = new StringBuilder();
        foreach (var item in ServiceDic.Values)
        {
            outputBuilder.AppendLine(item);
        }

        return outputBuilder.ToString().Trim();
    }

    public string GetSettingsPageOptions()
    {
        StringBuilder outputBuilder = new StringBuilder();
        int index = 0;
        foreach (var item in SettingsPageOptionsDic.Values)
        {
            if (index == 0)
            {
                outputBuilder.AppendLine(item);
            }
            else
            {
                outputBuilder.AppendLine($"{item}");
            }
            index++;
        }

        return outputBuilder.ToString();
    }

    public string GetGeneralSettingsPageOptions()
    {
        if (GeneralSettingsPageOptionsDic.Count == 0)
        {
            return "";
        }

        StringBuilder outputBuilder = new StringBuilder();
        int index = 0;
        foreach (var item in GeneralSettingsPageOptionsDic.Values)
        {
            if (index == 0)
            {
                outputBuilder.AppendLine(item);
            }
            else
            {
                outputBuilder.AppendLine($"{item}");
            }
            index++;
        }

        return outputBuilder.ToString();
    }

    public void ConfigAllMVVM(string safeProjectName)
    {
        if (UseContextMenu)
        {
            ServiceDic.Add(nameof(UseContextMenu), "services.AddSingleton<ContextMenuService>();");
        }

        if (UseGeneralSettingPage)
        {
            var generalCode = PredefinedCodes.GeneralSettingMVVMCode;
            generalCode = FixWithRealNamespace(generalCode, safeProjectName);
            SettingsPageOptionsDic.Add(nameof(UseGeneralSettingPage), generalCode);

            ServiceDic.Add(nameof(UseGeneralSettingPage), "services.AddTransient<GeneralSettingViewModel>();");
        }

        if (UseThemeSettingPage)
        {
            var themeCode = PredefinedCodes.ThemeSettingMVVMCode;
            themeCode = FixWithRealNamespace(themeCode, safeProjectName);
            SettingsPageOptionsDic.Add(nameof(UseThemeSettingPage), themeCode);
        }

        if (UseAppUpdatePage)
        {
            var appUpdateCode = PredefinedCodes.AppUpdateSettingMVVMCode;
            appUpdateCode = FixWithRealNamespace(appUpdateCode, safeProjectName);

            SettingsPageOptionsDic.Add(nameof(UseAppUpdatePage), appUpdateCode);

            ServiceDic.Add(nameof(UseAppUpdatePage), "services.AddTransient<AppUpdateSettingViewModel>();");
        }

        if (UseAboutPage)
        {
            var aboutCode = PredefinedCodes.AboutSettingMVVMCode;
            aboutCode = FixWithRealNamespace(aboutCode, safeProjectName);
            SettingsPageOptionsDic.Add(nameof(UseAboutPage), aboutCode);
            ServiceDic.Add(nameof(UseAboutPage), "services.AddTransient<AboutUsSettingViewModel>();");
        }

        if (UseHomeLandingPage)
        {
            ConfigJsonDic.Add(nameof(UseHomeLandingPage), ".ConfigureDefaultPage(typeof(HomeLandingPage))");
        }

        if (UseSettingsPage)
        {
            ConfigJsonDic.Add(nameof(UseSettingsPage), ".ConfigureSettingsPage(typeof(SettingsPage))");
        }

        if (SettingsPageOptionsDic.Count == 0)
        {
            var commentCode = PredefinedCodes.SettingsCardMVVMCommentCode;
            commentCode = FixWithRealNamespace(commentCode, safeProjectName);
            SettingsPageOptionsDic.Add("comment", commentCode);
        }
    }

    private string FixWithRealNamespace(string content, string safeProjectName)
    {
        return content?.Replace("$safeprojectname$", safeProjectName);
    }
    public void ConfigAll(string safeProjectName)
    {
        if (UseGeneralSettingPage)
        {
            var generalCode = PredefinedCodes.GeneralSettingCode;
            generalCode = FixWithRealNamespace(generalCode, safeProjectName);
            SettingsPageOptionsDic.Add(nameof(UseGeneralSettingPage), generalCode);
        }

        if (UseThemeSettingPage)
        {
            var themeCode = PredefinedCodes.ThemeSettingCode;
            themeCode = FixWithRealNamespace(themeCode, safeProjectName);
            SettingsPageOptionsDic.Add(nameof(UseThemeSettingPage), themeCode);
        }

        if (UseAppUpdatePage)
        {
            var appUpdateCode = PredefinedCodes.AppUpdateSettingCode;
            appUpdateCode = FixWithRealNamespace(appUpdateCode, safeProjectName);
            SettingsPageOptionsDic.Add(nameof(UseAppUpdatePage), appUpdateCode);
        }

        if (UseAboutPage)
        {
            var aboutCode = PredefinedCodes.AboutSettingCode;
            aboutCode = FixWithRealNamespace(aboutCode, safeProjectName);

            SettingsPageOptionsDic.Add(nameof(UseAboutPage), aboutCode);
        }

        if (UseHomeLandingPage)
        {
            ConfigJsonDic.Add(nameof(UseHomeLandingPage), ".ConfigureDefaultPage(typeof(HomeLandingPage));");
        }

        if (UseSettingsPage)
        {
            ConfigJsonDic.Add(nameof(UseSettingsPage), ".ConfigureSettingsPage(typeof(SettingsPage));");
        }

        if (SettingsPageOptionsDic.Count == 0)
        {
            var commentCode = PredefinedCodes.SettingsCardCommentCode;
            commentCode = FixWithRealNamespace(commentCode, safeProjectName);
            SettingsPageOptionsDic.Add("comment", commentCode);
        }
    }

    public void ConfigGeneral()
    {
        if (UseSettingsPage && UseGeneralSettingPage && UseDeveloperModeSetting && !UseJsonSetting)
        {
            GeneralSettingsPageOptionsDic.Add(nameof(UseDeveloperModeSetting), Environment.NewLine + PredefinedCodes.DeveloperModeSettingCode);
        }
        else if (UseSettingsPage && UseGeneralSettingPage && UseDeveloperModeSetting && UseJsonSetting)
        {
            GeneralSettingsPageOptionsDic.Add(nameof(UseDeveloperModeSetting), Environment.NewLine + PredefinedCodes.DeveloperModeSettingCode2);
        }
    }
}
