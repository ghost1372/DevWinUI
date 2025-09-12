﻿namespace DevWinUI_Template;

public static class PredefinedCodes
{
    public static string Windows11ContextMenuInitializer =
""""
if (RuntimeHelper.IsPackaged())
{
    ContextMenuItem menu = new ContextMenuItem
    {
        Title = "Open $projectname$ Here",
        Param = @"""{path}""",
        AcceptFileFlag = (int)FileMatchFlagEnum.All,
        AcceptDirectoryFlag = (int)(DirectoryMatchFlagEnum.Directory | DirectoryMatchFlagEnum.Background | DirectoryMatchFlagEnum.Desktop),
        AcceptMultipleFilesFlag = (int)FilesMatchFlagEnum.Each,
        Index = 0,
        Enabled = true,
        Icon = ProcessInfoHelper.GetFileVersionInfo().FileName,
        Exe = "$projectname$.exe"
    };

    ContextMenuService menuService = new ContextMenuService();
    await menuService.SaveAsync(menu);
}
"""";
    public static string Windows11ContextMenuMVVMInitializer =
""""
var menuService = GetService<ContextMenuService>();
if (menuService != null && RuntimeHelper.IsPackaged())
{
    ContextMenuItem menu = new ContextMenuItem
    {
        Title = "Open $projectname$ Here",
        Param = @"""{path}""",
        AcceptFileFlag = (int)FileMatchFlagEnum.All,
        AcceptDirectoryFlag = (int)(DirectoryMatchFlagEnum.Directory | DirectoryMatchFlagEnum.Background | DirectoryMatchFlagEnum.Desktop),
        AcceptMultipleFilesFlag = (int)FilesMatchFlagEnum.Each,
        Index = 0,
        Enabled = true,
        Icon = ProcessInfoHelper.GetFileVersionInfo().FileName,
        Exe = "$projectname$.exe"
    };

    await menuService.SaveAsync(menu);
}
"""";

    public static readonly string SettingsCardCommentCode =
"""
<!-- <dev:SettingsCard Description="Your Description"
                              Header="Your Header"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/AppIcon.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:MySettingPage, BreadCrumbHeader='My Setting'}" /> -->
""";

    public static readonly string SettingsCardMVVMCommentCode =
"""
<!-- <dev:SettingsCard Description="Your Description"
                              Header="Your Header"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/AppIcon.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:MySettingPage, BreadCrumbHeader='My Setting'}" /> -->
""";
    public static readonly string AboutSettingCode =
"""
<dev:SettingsCard Description="About $safeprojectname$ and Developer"
                              Header="About us"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Info.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:AboutUsSettingPage, BreadCrumbHeader='About us'}" />
""";
    public static readonly string AboutSettingMVVMCode =
"""
<dev:SettingsCard Description="About $safeprojectname$ and Developer"
                              Header="About us"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Info.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:AboutUsSettingPage, BreadCrumbHeader='About us'}" />
""";

    public static readonly string AppUpdateSettingCode =
"""
<dev:SettingsCard Description="Check for Updates"
                              Header="Update App"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Update.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:AppUpdateSettingPage, BreadCrumbHeader='Update App'}" />
""";

    public static readonly string AppUpdateSettingMVVMCode =
"""
<dev:SettingsCard Description="Check for Updates"
                              Header="Update App"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Update.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:AppUpdateSettingPage, BreadCrumbHeader='Update App'}" />
""";

    public static readonly string GeneralSettingCode =
"""
<dev:SettingsCard Description="Change your app Settings"
                              Header="General"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/General.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:GeneralSettingPage, BreadCrumbHeader='General'}" />
""";

    public static readonly string GeneralSettingMVVMCode =
"""
<dev:SettingsCard Description="Change your app Settings"
                              Header="General"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/General.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:GeneralSettingPage, BreadCrumbHeader='General'}" />
""";

    public static readonly string ThemeSettingCode =
"""
<dev:SettingsCard Description="Explore the different ways to customize the appearance and behavior of your app. You can change the material, theme, accent, and more options to suit your style and preference."
                              Header="Appearance &amp; behavior"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Theme.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:ThemeSettingPage, BreadCrumbHeader='Appearance &amp; behavior'}" />
""";

    public static readonly string ThemeSettingMVVMCode =
"""
<dev:SettingsCard Description="Explore the different ways to customize the appearance and behavior of your app. You can change the material, theme, accent, and more options to suit your style and preference."
                              Header="Appearance &amp; behavior"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Theme.png}"
                              IsClickEnabled="True"
                              Command="{x:Bind local:App.Current.NavService.NavigateToCommand}"
                              CommandParameter="{dev:NavigationParameter PageType=views:ThemeSettingPage, BreadCrumbHeader='Appearance &amp; behavior'}" />
""";

    public static readonly string StartupAppSettingCode =
"""
            <dev:SettingsCard Description="Automatically launch app when you log in to Windows"
                              Header="Run at startup"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/Startup.png}">
                <ToggleSwitch IsOn="{x:Bind dev:StartupHelper.IsAppStartupWithWindowsForXamlBindingEnabled, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
            </dev:SettingsCard>
""";

    public static readonly string DeveloperModeSettingCode =
"""
            <dev:SettingsCard Description="Help us improve this app by sharing optional diagnostics data to inform bug fixes, performance, and feature enhancements"
                              Header="Diagnostics &amp; feedback (Restart Required)"
                              HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/DevMode.png}">
                <ToggleSwitch />
            </dev:SettingsCard>
""";
    public static readonly string DeveloperModeSettingCode2 =
"""
            <dev:SettingsExpander Description="Help us improve this app by sharing optional diagnostics data to inform bug fixes, performance, and feature enhancements"
                                  Header="Diagnostics &amp; feedback (Restart Required)"
                                  HeaderIcon="{dev:BitmapIcon Source=Assets/Fluent/DevMode.png}">
                <ToggleSwitch IsOn="{x:Bind common:AppHelper.Settings.UseDeveloperMode, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
                <dev:SettingsExpander.ItemsHeader>
                    <HyperlinkButton HorizontalAlignment="Stretch"
                                     HorizontalContentAlignment="Left"
                                     Click="NavigateToLogPath_Click"
                                     Content="{x:Bind common:Constants.LogDirectoryPath}" />
                </dev:SettingsExpander.ItemsHeader>
            </dev:SettingsExpander>
""";
    public static readonly string GoToLogPathEvent =
""""
        private async void NavigateToLogPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = (sender as HyperlinkButton).Content.ToString();
            if (Directory.Exists(folderPath))
            {
                Windows.Storage.StorageFolder folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(folderPath);
                await Windows.System.Launcher.LaunchFolderAsync(folder);
            }
        }
"""";

    public static readonly string StartupTask =
""""
      <Extensions>
        <uap5:Extension Category="windows.startupTask">
          <uap5:StartupTask TaskId="$safeprojectname$StartOnLoginTask" Enabled="false" DisplayName="$safeprojectname$" />
        </uap5:Extension>
      </Extensions>
"""";

    public static readonly string StartupTaskInContextMenu =
""""
        <uap5:Extension Category="windows.startupTask">
          <uap5:StartupTask TaskId="$safeprojectname$StartOnLoginTask" Enabled="false" DisplayName="$safeprojectname$" />
        </uap5:Extension>
"""";
}
