using DevWinUI_Template.Common;
using DevWinUI_Template.Models;
using EnvDTE80;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace DevWinUI_Template;

public partial class NugetPackagePage : Page
{
    public ObservableCollection<NugetPackageModel> NuGetPackages { get; } = new ObservableCollection<NugetPackageModel>
    {
        new NugetPackageModel { Title = "CommunityToolkit.HighPerformance", PackageName = "CommunityToolkit.HighPerformance" },
        new NugetPackageModel { Title = "CommunityToolkit.Common", PackageName = "CommunityToolkit.Common" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Behaviors", PackageName = "CommunityToolkit.WinUI.Behaviors" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Extensions", PackageName = "CommunityToolkit.WinUI.Extensions" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Helpers", PackageName = "CommunityToolkit.WinUI.Helpers" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Triggers", PackageName = "CommunityToolkit.WinUI.Triggers" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Converters", PackageName = "CommunityToolkit.WinUI.Converters" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Animations", PackageName = "CommunityToolkit.WinUI.Animations" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Media", PackageName = "CommunityToolkit.WinUI.Media" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Collections", PackageName = "CommunityToolkit.WinUI.Collections" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Lottie", PackageName = "CommunityToolkit.WinUI.Lottie" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.Segmented", PackageName = "CommunityToolkit.WinUI.Controls.Segmented" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.Primitives", PackageName = "CommunityToolkit.WinUI.Controls.Primitives" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.Sizers", PackageName = "CommunityToolkit.WinUI.Controls.Sizers" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.HeaderedControls", PackageName = "CommunityToolkit.WinUI.Controls.HeaderedControls" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.RangeSelector", PackageName = "CommunityToolkit.WinUI.Controls.RangeSelector" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.ImageCropper", PackageName = "CommunityToolkit.WinUI.Controls.ImageCropper" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.RichSuggestBox", PackageName = "CommunityToolkit.WinUI.Controls.RichSuggestBox" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.RadialGauge", PackageName = "CommunityToolkit.WinUI.Controls.RadialGauge" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.CameraPreview", PackageName = "CommunityToolkit.WinUI.Controls.CameraPreview" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.TokenizingTextBox", PackageName = "CommunityToolkit.WinUI.Controls.TokenizingTextBox" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.LayoutTransformControl", PackageName = "CommunityToolkit.WinUI.Controls.LayoutTransformControl" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.ColorPicker", PackageName = "CommunityToolkit.WinUI.Controls.ColorPicker" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.TabbedCommandBar", PackageName = "CommunityToolkit.WinUI.Controls.TabbedCommandBar" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.SettingsControls", PackageName = "CommunityToolkit.WinUI.Controls.SettingsControls" },
        new NugetPackageModel { Title = "CommunityToolkit.WinUI.Controls.MetadataControl", PackageName = "CommunityToolkit.WinUI.Controls.MetadataControl" },

        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore", PackageName = "Microsoft.EntityFrameworkCore" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Sqlite", PackageName = "Microsoft.EntityFrameworkCore.Sqlite" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.SqlServer", PackageName = "Microsoft.EntityFrameworkCore.SqlServer" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Cosmos", PackageName = "Microsoft.EntityFrameworkCore.Cosmos" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.InMemory", PackageName = "Microsoft.EntityFrameworkCore.InMemory" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Relational", PackageName = "Microsoft.EntityFrameworkCore.Relational" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Abstractions", PackageName = "Microsoft.EntityFrameworkCore.Abstractions" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Analyzers", PackageName = "Microsoft.EntityFrameworkCore.Analyzers" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Design", PackageName = "Microsoft.EntityFrameworkCore.Design" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Proxies", PackageName = "Microsoft.EntityFrameworkCore.Proxies" },
        new NugetPackageModel { Title = "Microsoft.EntityFrameworkCore.Tools", PackageName = "Microsoft.EntityFrameworkCore.Tools" },

        new NugetPackageModel { Title = "Microsoft.Windows.SDK.BuildTools.MSIX", PackageName = "Microsoft.Windows.SDK.BuildTools.MSIX" },
        new NugetPackageModel { Title = "WinUI.Dock", PackageName = "WinUI.Dock" },
        new NugetPackageModel { Title = "ComputeSharp.WinUI", PackageName = "ComputeSharp.WinUI" },
        new NugetPackageModel { Title = "ComputeSharp.D2D1.WinUI", PackageName = "ComputeSharp.D2D1.WinUI" },
        new NugetPackageModel { Title = "Config.Net", PackageName = "Config.Net" },
        new NugetPackageModel { Title = "messagepack", PackageName = "messagepack" },
        new NugetPackageModel { Title = "NotifyIconEx", PackageName = "NotifyIconEx" },
        new NugetPackageModel { Title = "Ulid", PackageName = "Ulid" },
        new NugetPackageModel { Title = "WinUI.TableView", PackageName = "WinUI.TableView" },
        new NugetPackageModel { Title = "Microsoft.Windows.CsWinRT", PackageName = "Microsoft.Windows.CsWinRT" },
        new NugetPackageModel { Title = "Microsoft.Windows.CsWin32", PackageName = "Microsoft.Windows.CsWin32" },
        new NugetPackageModel { Title = "WinUIEx", PackageName = "WinUIEx" },
        new NugetPackageModel { Title = "Microsoft.Graphics.Win2D", PackageName = "Microsoft.Graphics.Win2D" },
        new NugetPackageModel { Title = "Newtonsoft.Json", PackageName = "Newtonsoft.Json" },
        new NugetPackageModel { Title = "HtmlAgilityPack", PackageName = "HtmlAgilityPack" },
        new NugetPackageModel { Title = "Downloader", PackageName = "Downloader" },
        new NugetPackageModel { Title = "Microsoft.Win32.Registry", PackageName = "Microsoft.Win32.Registry" },
        new NugetPackageModel { Title = "YamlDotNet", PackageName = "YamlDotNet" },
        new NugetPackageModel { Title = "System.Drawing.Common", PackageName = "System.Drawing.Common" },
        new NugetPackageModel { Title = "System.Management", PackageName = "System.Management" },
        new NugetPackageModel { Title = "SharpCompress", PackageName = "SharpCompress" },
        new NugetPackageModel { Title = "RestSharp", PackageName = "RestSharp" },
        new NugetPackageModel { Title = "Vanara.Windows.Shell", PackageName = "Vanara.Windows.Shell" },
        new NugetPackageModel { Title = "protobuf-net", PackageName = "protobuf-net" },
        new NugetPackageModel { Title = "protobuf-net.Core", PackageName = "protobuf-net.Core" },
        new NugetPackageModel { Title = "Humanizer.Core", PackageName = "Humanizer.Core" },
        new NugetPackageModel { Title = "MSTest.TestAdapter", PackageName = "MSTest.TestAdapter" },
        new NugetPackageModel { Title = "MSTest.TestFramework", PackageName = "MSTest.TestFramework" },
        new NugetPackageModel { Title = "Microsoft.TestPlatform.TestHost", PackageName = "Microsoft.TestPlatform.TestHost" },

        new NugetPackageModel { Title = "DevWinUI.ContextMenu", PackageName = "DevWinUI.ContextMenu" },
        new NugetPackageModel { Title = "DevWinUI.Shader", PackageName = "DevWinUI.Shader" },

        new NugetPackageModel { Title = "Serilog", PackageName = "Serilog", Tag = "Auto Implement", HasImplementation = true },
        new NugetPackageModel { Title = "Serilog.Sinks.File", PackageName = "Serilog.Sinks.File", Tag = "Auto Implement", HasImplementation = true },
        new NugetPackageModel { Title = "Serilog.Sinks.Debug", PackageName = "Serilog.Sinks.Debug", Tag = "Auto Implement", HasImplementation = true },
        new NugetPackageModel { Title = "Serilog.Sinks.Console", PackageName = "Serilog.Sinks.Console" },
        new NugetPackageModel { Title = "log4net", PackageName = "log4net" },
        new NugetPackageModel { Title = "NLog", PackageName = "NLog" },

        new NugetPackageModel { Title = "CommunityToolkit.Mvvm", PackageName = "CommunityToolkit.Mvvm" },
        new NugetPackageModel { Title = "Microsoft.Xaml.Behaviors.WinUI.Managed", PackageName = "Microsoft.Xaml.Behaviors.WinUI.Managed" },
        new NugetPackageModel { Title = "Microsoft.Extensions.Hosting", PackageName = "Microsoft.Extensions.Hosting" },
        new NugetPackageModel { Title = "Microsoft.Extensions.DependencyInjection", PackageName = "Microsoft.Extensions.DependencyInjection" },
        new NugetPackageModel { Title = "Microsoft.Extensions.Logging", PackageName = "Microsoft.Extensions.Logging" },
        new NugetPackageModel { Title = "Microsoft.Extensions.Configuration", PackageName = "Microsoft.Extensions.Configuration" },
    };
    public NugetPackagePage()
    {
        InitializeComponent();

        DataContext = WizardConfig.Current;

        var nPackages = new ObservableCollection<NugetPackageModel>(
            NuGetPackages.OrderBy(p => p.Title).ToList()
        );

        PackagesLV.ItemsSource = nPackages;

        Helper.UpdateCornerRadius(PackagesLV);
    }

    private void PackagesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView listView)
        {
            Helper.UpdateActiveCornerRadius(listView);
        }

        var opts = WizardConfig.Current;

        var selectedItems = PackagesLV.SelectedItems.OfType<NugetPackageModel>();

        opts.NuGetPackages = selectedItems.ToList();
    }
}
