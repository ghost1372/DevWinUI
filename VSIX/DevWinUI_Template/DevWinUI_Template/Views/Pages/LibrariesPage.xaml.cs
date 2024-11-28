using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public partial class LibrariesPage : Page
{
    public static LibrariesPage Instance { get; private set; }
    List<Library> items = new List<Library>();

    public LibrariesPage()
    {
        InitializeComponent();
        Instance = this;
        CreateBoxes();

        lvLibraries.ItemsSource = items;

        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLibraries.ItemsSource);
        PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
        view.GroupDescriptions.Add(groupDescription);
        UpdateHeader();
    }
    private void UpdateHeader()
    {
        LibraryColumn.Header = $"Library Name     |       {WizardConfig.LibraryDic?.Count} Item(s) selected";
    }
    public void CreateBoxes(List<Library> libraries)
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

            var option = new Library();

            if (string.IsNullOrEmpty(libVersion2))
            {
                option.Name = lib.Name;
            }
            else
            {
                option.Name = $"{lib.Name} - {libVersion2}";
            }

            option.Group = lib.Group;
            option.Net9Version = lib.Net9Version;
            option.IncludePreRelease = lib.IncludePreRelease;
            option.Version = lib.Version;

            items.Add(option);
        }
    }
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is Library lib)
        {
            WizardConfig.LibraryDic.AddIfNotExists(lib.Name, new Library
            {
                Name = lib.Name,
                Group = lib.Group,
                IncludePreRelease = lib.IncludePreRelease,
                Version = lib.Version,
                Net9Version = lib.Net9Version
            });
            UpdateHeader();
        }
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is Library lib)
        {
            WizardConfig.LibraryDic.Remove(lib.Name);
            UpdateHeader();
        }
    }

    public void CreateBoxes()
    {
        items.Clear();
        WizardConfig.LibraryDic = new();
        CreateBoxes(PreDefinedLibrary.InitDevWinUI());
        CreateBoxes(PreDefinedLibrary.InitUseful());
        CreateBoxes(PreDefinedLibrary.InitEFCore());
        CreateBoxes(PreDefinedLibrary.InitCommunityToolkit());
        CreateBoxes(PreDefinedLibrary.InitMVVM());
        CreateBoxes(PreDefinedLibrary.InitLog());
    }

    private void lvUsers_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Grid grid && grid.Children[0] is CheckBox check)
        {
            check.IsChecked = !check.IsChecked;
        }
    }
}
