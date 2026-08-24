using DevWinUI_Template.Views;
using DevWinUI_Template.Views.Startup;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevWinUI_Template;

internal static class WizardHelper
{
    public static async void AddSolutionFolder(Solution2 solution)
    {
        if (WizardConfig.Current.UseSolutionFolder)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var solutionFolder = solution.AddSolutionFolder(WizardConfig.Current.SolutionFolderName);
            if (solutionFolder != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                foreach (var item in WizardConfig.Current.SolutionFiles)
                {
                    solutionFolder.ProjectItems.AddFromFile(item.Value);
                }

                WizardConfig.Current.SolutionFiles.Clear();
            }
        }
    }

    public static async void AddGithubActionFile(Project project, _DTE _dte, string vsixRootFolderPath, string solutionDirectory)
    {
        if (WizardConfig.Current.UseGithubWorkflowFile)
        {
            var inputFile = vsixRootFolderPath + @"\Files\dotnet-release.yml";
            string outputDir = solutionDirectory + @"\.github\workflows\";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var outputFile = outputDir + "dotnet-release.yml";
            WizardConfig.Current.SolutionFiles.AddIfNotExists("workflow", outputFile);
            CopyFileToDestination(_dte, inputFile, outputFile);

            if (File.Exists(outputFile))
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var fileContent = File.ReadAllText(outputFile);
                fileContent = fileContent.Replace("YOUR_Folder/YOUR_APP_NAME.csproj", project.UniqueName);
                fileContent = fileContent.Replace("YOUR_APP_NAME", project.Name);
                File.WriteAllText(outputFile, fileContent);
            }
        }
    }
    public static void AddXamlStylerConfigFile(_DTE _dte, string vsixRootFolderPath, string solutionDirectory)
    {
        if (WizardConfig.Current.UseXamlStylerFile)
        {
            var inputFile = vsixRootFolderPath + @"\Files\settings.xamlstyler";

            var outputFile = solutionDirectory + @"\settings.xamlstyler";
            WizardConfig.Current.SolutionFiles.AddIfNotExists("XamlStyler", outputFile);
            CopyFileToDestination(_dte, inputFile, outputFile);
        }
    }

    public static void AddEditorConfigFile(_DTE _dte, string vsixRootFolderPath, string solutionDirectory)
    {
        if (WizardConfig.Current.UseEditorConfigFile)
        {
            var inputFile = vsixRootFolderPath + @"\Files\.editorconfig";

            var outputFile = solutionDirectory + @"\.editorconfig";
            WizardConfig.Current.SolutionFiles.AddIfNotExists("EditorConfig", outputFile);
            CopyFileToDestination(_dte, inputFile, outputFile);
        }
    }

    public static async void OpenStartupToolWindow()
    {
        if (!WizardConfig.Current.ShowStartupToolWindow)
        {
            return;
        }

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

    public static async void CopyFileToDestination(_DTE _dte, string inputfile, string outputfile)
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
    public static async Task<(string VSIXRootFolder, string ProjectTemplatesFolder, string VSTemplatePath)> GetVSIXPathAsync(_DTE _dte, string vstemplateName)
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
    public static void SaveAllProjects()
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
}
