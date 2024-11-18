using System.Collections.Generic;

namespace DevWinUI_Template.WizardUI
{
    public static class WizardConfig
    {
        public static readonly string SolutionFolderNameDefault = "Solution Items";
        public static readonly string MinimumTargetPlatformDefault = "17763";

        public static Dictionary<string, Library> LibraryDic = new Dictionary<string, Library>();
        public static Dictionary<string, string> CSProjectElements = new Dictionary<string, string>();
        public static Dictionary<string, string> UnvirtualizedResources = new Dictionary<string, string>();

        public static string DotNetVersion = "net8.0";
        public static string TargetFrameworkVersion = "22621";
        public static string MinimumTargetPlatform = "17763";
        public static string Platforms = "x86;x64;ARM64";
        public static string RuntimeIdentifiers = "win-x86;win-x64;win-arm64";
        public static string SolutionFolderName = "Solution Items";
        public static string TrimMode = "partial";
        public static string Nullable = "disable";
        
        public static bool UseGithubWorkflowFile;
        public static bool UseXamlStylerFile;

        public static bool UseEditorConfigFile = true;
        public static bool UseJsonSettings = true;
        public static bool UseSolutionFolder = true;
        public static bool PublishAot = false;

        public static bool IsUnPackagedMode;
        public static bool IsBlank;
        public static bool HasPages;
        public static bool UseHomeLandingPage;
        public static bool UseSettingsPage;
        public static bool UseGeneralSettingPage;
        public static bool UseThemeSettingPage;
        public static bool UseAppUpdatePage;
        public static bool UseAboutPage;
        public static bool UsePreReleaseVersion;
        public static bool UseDeveloperModeSetting;
        public static bool UseColorsDic;
        public static bool UseStylesDic;
        public static bool UseConvertersDic;
        public static bool UseFontsDic;
        public static bool UseWindow11ContextMenu;
    }
}
