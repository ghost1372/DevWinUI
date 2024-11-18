using System.Collections.Generic;
using System.Windows.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template
{
    public partial class LibrariesPage : Page
    {
        public static LibrariesPage Instance { get; private set; }
        public LibrariesPage()
        {
            InitializeComponent();
            Instance = this;
            CreateBoxes();
        }

        public void CreateBoxes(List<Library> libraries, Panel panel)
        {
            foreach (var lib in libraries)
            {
                string libVersion = lib.Version;
                string libVersion2 = lib.Version;
                if (WizardConfig.DotNetVersion.Contains("net9") && !string.IsNullOrEmpty(lib.Net9Version))
                {
                    libVersion = lib.Net9Version;
                    libVersion2 = lib.Net9Version;
                }

                if (!lib.IncludePreRelease)
                {
                    lib.IncludePreRelease = WizardConfig.UsePreReleaseVersion;
                }

                var option = new LibraryOptionUC();

                if (string.IsNullOrEmpty(libVersion2))
                {
                    option.Title = lib.Name;
                }
                else
                {
                    option.Title = $"{lib.Name} - {libVersion2}";
                }

                option.Checked += (s, e) =>
                {
                    WizardConfig.LibraryDic.AddIfNotExists(lib.Name, new Library(lib.Name, lib.IncludePreRelease));
                };

                option.Unchecked += (s, e) =>
                {
                    WizardConfig.LibraryDic.Remove(lib.Name);
                };

                panel.Children.Add(option);
            }
        }

        public void CreateBoxes()
        {
            if (DevWinUIPanel != null)
            {
                WizardConfig.LibraryDic = new();
                DevWinUIPanel.Children.Clear();
                GeneralPanel.Children.Clear();
                EFCorePanel.Children.Clear();
                CommunityToolkitPanel.Children.Clear();
                MVVMPanel.Children.Clear();
                LogPanel.Children.Clear();

                CreateBoxes(PreDefinedLibrary.InitDevWinUI(), DevWinUIPanel);
                CreateBoxes(PreDefinedLibrary.InitUseful(), GeneralPanel);
                CreateBoxes(PreDefinedLibrary.InitEFCore(), EFCorePanel);
                CreateBoxes(PreDefinedLibrary.InitCommunityToolkit(), CommunityToolkitPanel);
                CreateBoxes(PreDefinedLibrary.InitMVVM(), MVVMPanel);
                CreateBoxes(PreDefinedLibrary.InitLog(), LogPanel);
            }
        }
    }
}
