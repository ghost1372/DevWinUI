using DevWinUI_Template.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace DevWinUI_Template;

public partial class PackageModePage : Page
{
    public ObservableCollection<PackageModeModel> PackageItems { get; } = new ObservableCollection<PackageModeModel>
    {
        new PackageModeModel { Value = "MSIX", Title = "MSIX (Packaged)", Subtitle = "Recommended for microsoft store and modern deployment.", Description = "✔️ Full app lifecycle management\n✔️ Better security and isolation\n✔️ Support automatic updates", Icon = new SymbolIcon { FontSize = 48, Symbol = SymbolRegular.BoxMultiple24 } },
        new PackageModeModel { Value = "None" ,Title = "Unpackaged", Subtitle = "distribute as a loose file application.", Description = "✔️ Simple deployment\n✔️ No package identity\n✔️ Manual update management", Icon = new SymbolIcon { FontSize = 48, Symbol = SymbolRegular.Folder48 } },
    };
    public PackageModePage()
    {
        InitializeComponent();

        DataContext = WizardConfig.Current;

        PackageLV.ItemsSource = PackageItems;
        PackageLV.SelectedValue = WizardConfig.Current.WindowsPackageMode;
    }
}
