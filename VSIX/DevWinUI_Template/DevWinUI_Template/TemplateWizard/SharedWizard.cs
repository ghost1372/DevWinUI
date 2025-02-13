using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevWinUI_Template.Options;
using DevWinUI_Template.Views.Startup;
using DevWinUI_Template.WizardUI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio;

namespace DevWinUI_Template;

public class SharedWizard
{
    private Dictionary<string, string> solutionFiles = new();
    private bool _shouldAddProjectItem;
    private _DTE _dte;

    public bool UseFileLogger;
    public bool UseDebugLogger;

    public string VSTemplateFilePath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}\ProjectTemplates\CSharp\1033\{Template}\{Template}.vstemplate
    public string ProjectTemplatesFolderPath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}\ProjectTemplates\CSharp\1033\{Template}
    public string VSIXRootFolderPath; // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}

    public string ProjectName; // App
    public string SafeProjectName; // App
    public string SpecifiedSolutionName; // App
    public string SolutionDirectory; // E:\\source\\App
    public string DestinationDirectory;// E:\source\App\App

    private Project _project;
    private IComponentModel _componentModel;
    private List<Library> _nuGetPackages;
    private IVsNuGetProjectUpdateEvents _nugetProjectUpdateEvents;
    private IVsThreadedWaitDialog2 _waitDialog;
    public void ProjectFinishedGenerating(Project project)
    {
        _project = project;
    }

    public async void RunFinished()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var _solution = (Solution2)_dte.Solution;

        AddGithubActionFile(_project);
        AddXamlStylerConfigFile();

        AddSolutionFolder(_solution);

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

    private async void OpenStartupToolWindow()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        IVsShell shell = (IVsShell)Package.GetGlobalService(typeof(SVsShell));
        if (shell != null)
        {
            Guid packageGuid = new Guid(DevWinUI_TemplatePackage.PackageGuidString);
            IVsPackage package;
            shell.LoadPackage(ref packageGuid, out package);

            if (package != null)
            {
                ToolWindowPane window = ((DevWinUI_TemplatePackage)package).FindToolWindow(typeof(StartupToolWindow), 0, true);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

                // Ensure that the window docks in the central document area
                windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, (int)VSFRAMEMODE.VSFM_MdiChild);

                // Show the window
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }
    }

    private void OnSolutionRestoreFinished(IReadOnlyList<string> projects)
    {
        OpenStartupToolWindow();

        // Debouncing prevents multiple rapid executions of 'InstallNuGetPackageAsync'
        // during solution restore.
        if (_nugetProjectUpdateEvents == null)
        {
            return;
        }
        _nugetProjectUpdateEvents.SolutionRestoreFinished -= OnSolutionRestoreFinished;
        var joinableTaskFactory = new JoinableTaskFactory(ThreadHelper.JoinableTaskContext);
        _ = joinableTaskFactory.RunAsync(InstallNuGetPackagesAsync);
    }

    /// <summary>
    /// Get Parameters
    /// </summary>
    /// <param name="automationObject"></param>
    /// <param name="replacementsDictionary"></param>
    /// <param name="runKind"></param>
    /// <param name="customParams"></param>
    public async void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, TemplateConfig templateConfig)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        _componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
        _waitDialog = ServiceProvider.GlobalProvider.GetService(typeof(SVsThreadedWaitDialog)) as IVsThreadedWaitDialog2;
        if (_componentModel != null)
        {
            _nugetProjectUpdateEvents = _componentModel.GetService<IVsNuGetProjectUpdateEvents>();
            if (_nugetProjectUpdateEvents != null)
            {
                _nugetProjectUpdateEvents.SolutionRestoreFinished += OnSolutionRestoreFinished;
            }
        }

        ProjectName = replacementsDictionary["$projectname$"];
        SafeProjectName = replacementsDictionary["$safeprojectname$"];
        SpecifiedSolutionName = replacementsDictionary["$specifiedsolutionname$"];
        SolutionDirectory = replacementsDictionary["$solutiondirectory$"];
        DestinationDirectory = replacementsDictionary["$destinationdirectory$"];

        _shouldAddProjectItem = false;
        WizardConfig.HasPages = templateConfig.HasPages;
        WizardConfig.IsBlank = templateConfig.IsBlank;

        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        _dte = automationObject as _DTE;

        if (templateConfig.IsBlank || templateConfig.IsTest)
        {
            if (Helper.IsPreviewVSIX())
            {
                WizardConfig.UsePreReleaseVersion = true;
            }

            AddReplacementsDictionary(replacementsDictionary);
            _shouldAddProjectItem = true;
            return;
        }

        var inputForm = new MainWindow();
        var result = inputForm.ShowDialog();

        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        if (result.HasValue && result.Value)
        {
            AddReplacementsDictionary(replacementsDictionary);
            var vsix = await GetVSIXPathAsync(templateConfig.TemplateType.ToString());
            VSTemplateFilePath = vsix.VSTemplatePath;
            ProjectTemplatesFolderPath = vsix.ProjectTemplatesFolder;
            VSIXRootFolderPath = vsix.VSIXRootFolder;

            _shouldAddProjectItem = true;

            AddEditorConfigFile();

            var libs = WizardConfig.LibraryDic;

            #region Libs
            foreach (var lib in libs.Values)
            {
                _nuGetPackages?.Add(new Library(lib.Name, lib.IncludePreRelease));
            }

            if (WizardConfig.UseJsonSettings)
            {
                _nuGetPackages.Add(new Library("nucs.JsonSettings", WizardConfig.UsePreReleaseVersion));
                _nuGetPackages.Add(new Library("nucs.JsonSettings.AutoSaveGenerator", WizardConfig.UsePreReleaseVersion));
            }

            _nuGetPackages = _nuGetPackages.Distinct().ToList();
            #endregion

            #region CSProjectElements
            // Add CSProjectElements
            if (WizardConfig.CSProjectElements != null && WizardConfig.CSProjectElements.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var entity in WizardConfig.CSProjectElements)
                {
                    sb.AppendLine($"{entity.Value}");
                }

                replacementsDictionary.Add("$CustomCSProjectElement$", Environment.NewLine + $"{sb.ToString().Trim()}");
            }
            else
            {
                replacementsDictionary.Add("$CustomCSProjectElement$", "");
            }

            #endregion

            #region AppxManifest
            if (WizardConfig.UnvirtualizedResources != null && WizardConfig.UnvirtualizedResources.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var entity in WizardConfig.UnvirtualizedResources)
                {
                    sb.AppendLine($"{entity.Value}");
                }

                replacementsDictionary.Add("$UnvirtualizedResourcesCapability$", Environment.NewLine + "    <rescap:Capability Name=\"unvirtualizedResources\" />");
                replacementsDictionary.Add("$UnvirtualizedResources$", Environment.NewLine + $"{sb.ToString().Trim()}");
                replacementsDictionary.Add("$AppxManifestDesktop6$", Environment.NewLine + "  xmlns:desktop6=\"http://schemas.microsoft.com/appx/manifest/desktop/windows10/6\"");
            }
            else
            {
                replacementsDictionary.Add("$UnvirtualizedResourcesCapability$", "");
                replacementsDictionary.Add("$UnvirtualizedResources$", "");
                replacementsDictionary.Add("$AppxManifestDesktop6$", "");
            }
            #endregion

            #region Add Xaml Dictionary if User Use Extra Lib
            #region Blank

            if (templateConfig.IsBlank)
            {
                if (libs != null && libs.ContainsKey(Constants.DevWinUI_Controls))
                {
                    replacementsDictionary.Add("$DevWinUI.Controls$", Environment.NewLine + Constants.DevWinUI_Controls_Xaml);
                }
                else
                {
                    replacementsDictionary.Add("$DevWinUI.Controls$", "");
                }
            }

            #endregion

            if (libs != null && libs.ContainsKey(Constants.DevWinUI_ContextMenu))
            {
                WizardConfig.UseWindow11ContextMenu = true;
                replacementsDictionary.Add("$CLSID$", Guid.NewGuid().ToString());
                var windows11ContextMenu = PredefinedCodes.Windows11ContextMenuInitializer;
                var windows11ContextMenuMVVM = PredefinedCodes.Windows11ContextMenuMVVMInitializer;
                windows11ContextMenu = windows11ContextMenu.Replace("$projectname$", ProjectName);
                windows11ContextMenuMVVM = windows11ContextMenuMVVM.Replace("$projectname$", ProjectName);
                replacementsDictionary.Add("$Windows11ContextMenuInitializer$", Environment.NewLine + windows11ContextMenu);
                replacementsDictionary.Add("$Windows11ContextMenuMVVMInitializer$", Environment.NewLine + Environment.NewLine + windows11ContextMenuMVVM);
            }
            else
            {
                WizardConfig.UseWindow11ContextMenu = false;
                replacementsDictionary.Add("$CLSID$", "");
                replacementsDictionary.Add("$Windows11ContextMenuInitializer$", "");
                replacementsDictionary.Add("$Windows11ContextMenuMVVMInitializer$", "");
            }

            #endregion

            if (templateConfig.HasNavigationView)
            {
                new ColorsDicOption().ConfigColorsDic(replacementsDictionary, WizardConfig.UseHomeLandingPage);
            }

            // Add Xaml Dictionary
            new DictionaryOption().ConfigDictionary(replacementsDictionary, templateConfig.HasNavigationView, WizardConfig.UseHomeLandingPage, WizardConfig.UseColorsDic, WizardConfig.UseStylesDic, WizardConfig.UseConvertersDic, WizardConfig.UseFontsDic);

            #region Codes
            var configCodes = new ConfigCodes(WizardConfig.UseAboutPage, WizardConfig.UseAppUpdatePage, WizardConfig.UseGeneralSettingPage, WizardConfig.UseHomeLandingPage, WizardConfig.UseSettingsPage, WizardConfig.UseThemeSettingPage, WizardConfig.UseDeveloperModeSetting, WizardConfig.UseJsonSettings, WizardConfig.UseWindow11ContextMenu, WizardConfig.UseStartupSetting);

            if (templateConfig.IsMVVM)
            {
                configCodes.ConfigAllMVVM(SafeProjectName);
            }
            else
            {
                configCodes.ConfigAll(SafeProjectName);
            }

            configCodes.ConfigGeneral();

            var configs = configCodes.GetConfigJson();
            var services = configCodes.GetServices();
            var settingsCards = configCodes.GetSettingsPageOptions();
            var generalSettingsCards = configCodes.GetGeneralSettingsPageOptions();

            if (configCodes.ConfigJsonDic.Count > 0)
            {
                string[] lines = configs.Split(new[] { "\r\n" }, StringSplitOptions.None);

                // Reapply consistent indentation
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = "                " + lines[i].Trim();
                }

                // Join the lines back together
                configs = string.Join("\n", lines);
                replacementsDictionary.Add("$ConfigDefaultPages$", "\n" + configs);
            }
            else
            {
                replacementsDictionary.Add("$ConfigDefaultPages$", "");
            }

            if (configCodes.ServiceDic.Count > 0)
            {
                replacementsDictionary.Add("$Services$", Environment.NewLine + services);
            }
            else
            {
                replacementsDictionary.Add("$Services$", "");
            }

            replacementsDictionary.Add("$SettingsCards$", settingsCards);

            #endregion

            #region Serilog
            var serilog = new SerilogOption();
            serilog.ConfigSerilog(replacementsDictionary, libs, WizardConfig.UseJsonSettings, WizardConfig.UseDeveloperModeSetting);
            UseFileLogger = serilog.UseFileLogger;
            UseDebugLogger = serilog.UseDebugLogger;

            if (!string.IsNullOrEmpty(generalSettingsCards))
            {
                replacementsDictionary.AddIfNotExists("$GeneralSettingsCards$", generalSettingsCards);
            }
            else
            {
                replacementsDictionary.AddIfNotExists("$GeneralSettingsCards$", "");
            }

            if (libs.ContainsKey("Serilog.Sinks.Debug") || libs.ContainsKey("Serilog.Sinks.File"))
            {
                if (WizardConfig.UseJsonSettings && WizardConfig.UseDeveloperModeSetting && WizardConfig.UseSettingsPage && WizardConfig.UseGeneralSettingPage)
                {
                    replacementsDictionary.AddIfNotExists("$GoToLogPathEvent$", Environment.NewLine + Environment.NewLine + PredefinedCodes.GoToLogPathEvent);
                    replacementsDictionary.AddIfNotExists("$DeveloperModeConfig$", Environment.NewLine + "private bool useDeveloperMode { get; set; }");
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
            #endregion

            #region Json Settings
            if (WizardConfig.UseJsonSettings)
            {
                if (templateConfig.IsMVVM)
                {
                    replacementsDictionary.Add("$AppUpdateMVVMGetDateTime$", Environment.NewLine + """LastUpdateCheck = Settings.LastUpdateCheck;""");
                    replacementsDictionary.Add("$AppUpdateMVVMSetDateTime$", Environment.NewLine + """Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();""");
                }
                else
                {
                    replacementsDictionary.Add("$AppUpdateGetDateTime$", Environment.NewLine + Environment.NewLine + """TxtLastUpdateCheck.Text = Settings.LastUpdateCheck;""");
                    replacementsDictionary.Add("$AppUpdateSetDateTime$", Environment.NewLine + """Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();""");
                }

                replacementsDictionary.Add("$AppConfigFilePath$", Environment.NewLine + """public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");""");

                if (WizardConfig.UseAppUpdatePage && WizardConfig.UseSettingsPage)
                {
                    replacementsDictionary.Add("$AppUpdateConfig$", Environment.NewLine + """private string lastUpdateCheck { get; set; }""");
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

            new GlobalUsingOption(replacementsDictionary, SafeProjectName, UseFileLogger, UseDebugLogger);

            WizardConfig.LibraryDic?.Clear();
            WizardConfig.CSProjectElements?.Clear();
        }
        else
        {
            inputForm.Close();
            throw new WizardBackoutException();
        }
    }

    private void AddReplacementsDictionary(Dictionary<string, string> replacementsDictionary)
    {
        // Add Base Library Versions
        replacementsDictionary.Add("$DotNetVersion$", WizardConfig.DotNetVersion.ToString());
        replacementsDictionary.Add("$TargetFrameworkVersion$", WizardConfig.TargetFrameworkVersion.ToString());
        replacementsDictionary.Add("$MinimumTargetPlatform$", WizardConfig.MinimumTargetPlatform.ToString());
        replacementsDictionary.Add("$Platforms$", WizardConfig.Platforms.ToString());
        replacementsDictionary.Add("$RuntimeIdentifiers$", WizardConfig.RuntimeIdentifiers.ToString());

        replacementsDictionary.Add("$Nullable$", WizardConfig.Nullable);
        replacementsDictionary.Add("$TrimMode$", WizardConfig.TrimMode);
        replacementsDictionary.Add("$PublishAot$", WizardConfig.PublishAot.ToString());

        replacementsDictionary.Add("$AddJsonSettings$", WizardConfig.UseJsonSettings.ToString());
        replacementsDictionary.Add("$AddEditorConfig$", WizardConfig.UseEditorConfigFile.ToString());
        replacementsDictionary.Add("$AddSolutionFolder$", WizardConfig.UseSolutionFolder.ToString());
        replacementsDictionary.Add("$AddHomeLandingPage$", WizardConfig.UseHomeLandingPage.ToString());
        replacementsDictionary.Add("$AddSettingsPage$", WizardConfig.UseSettingsPage.ToString());
        replacementsDictionary.Add("$AddGeneralSettingPage$", WizardConfig.UseGeneralSettingPage.ToString());
        replacementsDictionary.Add("$AddThemeSettingPage$", WizardConfig.UseThemeSettingPage.ToString());
        replacementsDictionary.Add("$AddAppUpdatePage$", WizardConfig.UseAppUpdatePage.ToString());
        replacementsDictionary.Add("$AddAboutPage$", WizardConfig.UseAboutPage.ToString());
        replacementsDictionary.Add("$T4_NAMESPACE$", SafeProjectName);

        #region IsUnPackaged
        if (WizardConfig.IsUnPackagedMode)
        {
            replacementsDictionary.Add("$WindowsPackageType$", "None");
            replacementsDictionary.Add("$PackagedAppTaskId$", "");
            replacementsDictionary.Add("$UAP5$", "");
            replacementsDictionary.Add("$StartupTask$", "");
        }
        else
        {
            replacementsDictionary.Add("$WindowsPackageType$", "MSIX");

            if (WizardConfig.UseGeneralSettingPage && WizardConfig.UseStartupSetting)
            {
                var content = PredefinedCodes.SetPackagedAppTaskId.Replace("$safeprojectname$", SafeProjectName);
                replacementsDictionary.Add("$PackagedAppTaskId$", Environment.NewLine + content);
                replacementsDictionary.Add("$UAP5$", Environment.NewLine + "  xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\"");

                if (WizardConfig.UseWindow11ContextMenu)
                {
                    var taskInContextMenuContent = PredefinedCodes.StartupTaskInContextMenu.Replace("$safeprojectname$", SafeProjectName);
                    replacementsDictionary.Add("$StartupTask$", Environment.NewLine + taskInContextMenuContent);
                }
                else
                {
                    var taskContent = PredefinedCodes.StartupTask.Replace("$safeprojectname$", SafeProjectName);
                    replacementsDictionary.Add("$StartupTask$", Environment.NewLine + taskContent);
                }

            }
            else
            {
                replacementsDictionary.Add("$PackagedAppTaskId$", "");
                replacementsDictionary.Add("$UAP5$", "");
                replacementsDictionary.Add("$StartupTask$", "");
            }
        }
        #endregion

        if (WizardConfig.UseWindow11ContextMenu)
        {
            replacementsDictionary.Add("$OnLaunchedAsyncKeyword$", "async ");
        }
        else
        {
            replacementsDictionary.Add("$OnLaunchedAsyncKeyword$", "");
        }

        #region Libs
        // Assuming package list is passed via a custom parameter in the .vstemplate file
        if (replacementsDictionary.TryGetValue("$NuGetPackages$", out string packages))
        {
            _nuGetPackages = new();
            var basePackages = packages.Split(';').Where(p => !string.IsNullOrEmpty(p));
            foreach (var baseItem in basePackages)
            {
                _nuGetPackages.Add(new Library(baseItem, WizardConfig.UsePreReleaseVersion));
            }
        }
        #endregion
    }
    public bool ShouldAddProjectItem()
    {
        return _shouldAddProjectItem;
    }

    public async void AddSolutionFolder(Solution2 solution)
    {
        if (WizardConfig.UseSolutionFolder)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var solutionFolder = solution.AddSolutionFolder(WizardConfig.SolutionFolderName);
            if (solutionFolder != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                foreach (var item in solutionFiles)
                {
                    solutionFolder.ProjectItems.AddFromFile(item.Value);
                }

                solutionFiles.Clear();
            }
        }
    }
    public void AddEditorConfigFile()
    {
        if (WizardConfig.UseEditorConfigFile)
        {
            var inputFile = VSIXRootFolderPath + @"\Files\.editorconfig";

            var outputFile = SolutionDirectory + @"\.editorconfig";
            solutionFiles.AddIfNotExists("EditorConfig", outputFile);
            CopyFileToDestination(inputFile, outputFile);
        }
    }
    public async void AddGithubActionFile(Project project)
    {
        if (WizardConfig.UseGithubWorkflowFile)
        {
            var inputFile = VSIXRootFolderPath + @"\Files\dotnet-release.yml";
            string outputDir = SolutionDirectory + @"\.github\workflows\";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var outputFile = outputDir + "dotnet-release.yml";
            solutionFiles.AddIfNotExists("workflow", outputFile);
            CopyFileToDestination(inputFile, outputFile);

            if (File.Exists(outputFile))
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var fileContent = File.ReadAllText(outputFile);
                fileContent = fileContent.Replace("YOUR_Folder/YOUR_APP_NAME.csproj", project.UniqueName);
                fileContent = fileContent.Replace("YOUR_APP_NAME", project.Name);
                var platforms = WizardConfig.Platforms.Replace(";", ", ");
                fileContent = fileContent.Replace("[x64, x86, arm64]", $"[{platforms}]");
                File.WriteAllText(outputFile, fileContent);
            }
        }
    }
    public void AddXamlStylerConfigFile()
    {
        if (WizardConfig.UseXamlStylerFile)
        {
            var inputFile = VSIXRootFolderPath + @"\Files\settings.xamlstyler";

            var outputFile = SolutionDirectory + @"\settings.xamlstyler";
            solutionFiles.AddIfNotExists("XamlStyler", outputFile);
            CopyFileToDestination(inputFile, outputFile);
        }
    }
    public async void CopyFileToDestination(string inputfile, string outputfile)
    {
        try
        {
            // Check if the file exists
            if (File.Exists(inputfile))
            {
                // Assuming 'outputfile' is the destination path
                string destinationPath = outputfile;

                // Copy the file
                File.Copy(inputfile, destinationPath, true);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                // Refresh the solution explorer to make sure the new file is visible
                _dte.ExecuteCommand("View.Refresh");
            }
            else
            {
                // Handle the case where the source file doesn't exist
                // Log or show an error message
            }
        }
        catch (Exception)
        {
            // Handle exceptions
            // Log or show an error message
        }
    }

    /// <summary>
    /// VSIXRootFolder: %AppData%\...\EXTENSIONS\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}
    /// ProjectTemplatesFolder: %AppData%\...\EXTENSIONS\Mahdi Hosseini\DevWinUI TEMPLATES FOR WINUI\{Version}\ProjectTemplates\CSharp\1033\{Template}
    /// ProjectTemplatesFolder: // %AppData%\...\Extensions\Mahdi Hosseini\DevWinUI Templates for WinUI\{Version}\ProjectTemplates\CSharp\1033\{Template}\{Template}.vstemplate
    /// </summary>
    /// <param name="vstemplateName">WinUIApp</param>
    /// <returns></returns>
    public async Task<(string VSIXRootFolder, string ProjectTemplatesFolder, string VSTemplatePath)> GetVSIXPathAsync(string vstemplateName)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        vstemplateName = vstemplateName?.Replace("_", "-");
        Solution2 soln = (Solution2)_dte.Solution;
        var vstemplateFileName = soln.GetProjectTemplate($"{vstemplateName}.vstemplate", "CSharp");

        string folderPath = Path.GetDirectoryName(vstemplateFileName);
        string projectTemplatesFolder = folderPath;
        while (folderPath.Contains("ProjectTemplates"))
        {
            folderPath = Directory.GetParent(folderPath).FullName;
        }

        return (folderPath, projectTemplatesFolder, vstemplateFileName);
    }

    private async Task InstallNuGetPackagesAsync()
    {
        await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            int canceled; // This variable will store the status after the dialog is closed

            // Start the package installation task but do not await it here
            var installationTask = StartInstallationAsync();

            // Start the threaded wait dialog
            _waitDialog.StartWaitDialog(null, "Installing NuGet packages into project...", null, null, "Operation in progress...", 0, false, true);

            // Now await the installation task to complete
            await installationTask;

            // Once the installation is complete, end the wait dialog
            _waitDialog.EndWaitDialog(out canceled);
            // Check if the process was canceled before proceeding
            if (canceled == 0) // If not canceled, finalize the process
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                SaveAllProjects();
            }
        });
    }

    private async Task StartInstallationAsync()
    {
        IVsPackageInstaller2 installer = _componentModel.GetService<IVsPackageInstaller2>();
        if (installer == null)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            LogError("Could not obtain IVsPackageInstaller service.");
            return;
        }

        // Process each package installation
        foreach (var packageId in _nuGetPackages)
        {
            try
            {
                if (NugetClientHelper.IsInternetAvailable())
                {
                    var packageMeta = await NugetClientHelper.GetPackageMetaDataAsync(packageId.Name, packageId.IncludePreRelease);
                    var isCacheAvailable = NugetClientHelper.IsCacheAvailableForPackage(packageId.Name, packageMeta.Identity.Version.ToString());

                    if (isCacheAvailable)
                    {
                        await Task.Run(() => installer.InstallLatestPackage(NugetClientHelper.globalPackagesFolder, _project, packageId.Name, packageId.IncludePreRelease, false));
                    }
                    else
                    {
                        try
                        {
                            await Task.Run(() => installer.InstallLatestPackage(null, _project, packageId.Name, packageId.IncludePreRelease, false));
                        }
                        catch (InvalidOperationException)
                        {
                            if (!packageId.IncludePreRelease)
                            {
                                await Task.Run(() => installer.InstallLatestPackage(null, _project, packageId.Name, true, false));
                            }
                        }
                    }
                }
                else
                {
                    await Task.Run(() => installer.InstallLatestPackage(NugetClientHelper.globalPackagesFolder, _project, packageId.Name, packageId.IncludePreRelease, false));
                }
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogError($"Failed to install NuGet package: {packageId}. Error: {ex.Message}");
            }
        }
    }

    private void SaveAllProjects()
    {
        ThreadHelper.ThrowIfNotOnUIThread("SaveAllProjects must be called on the UI thread.");

        var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
        if (dte != null && dte.Solution != null && dte.Solution.Projects != null)
        {
            foreach (Project project in dte.Solution.Projects)
            {
                if (project != null)
                {
                    project.Save();
                    VSDocumentHelper.FormatXmlBasedFile(project.FullName);
                }
            }
        }
    }
    private async void LogError(string message)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        ThreadHelper.JoinableTaskFactory.Run(async delegate
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsActivityLog log = ServiceProvider.GlobalProvider.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            if (log != null)
            {
                int hr = log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, ToString(), message);
            }
        });
    }
}
