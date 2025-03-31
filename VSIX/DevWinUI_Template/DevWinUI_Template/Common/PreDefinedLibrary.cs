using System.Collections.Generic;
using System.Linq;

namespace DevWinUI_Template;

public enum Group
{
    None,
    General,
    DevWinUI,
    CommunityToolkit,
    EntityFrameworkCore,
    Test,
    Log,
    MVVM
}
public class Library
{
    public string Name { get; set; }
    public Group Group { get; set; }
    public string Version { get; set; }
    public string Net9Version { get; set; }
    public bool IncludePreRelease { get; set; }
    public bool IsSelected { get; set; }
    public Library()
    {

    }
    public Library(string name, string version, string net9Version, Group group, bool includePreRelease = false)
    {
        Name = name;
        Group = group;
        Net9Version = net9Version;
        Version = version;
        IncludePreRelease = includePreRelease;
    }

    public Library(string name, Group group, bool includePreRelease = false)
    {
        Name = name;
        Group = group;
        IncludePreRelease = includePreRelease;
        Net9Version = null;
        Version = null;
    }
    public Library(string name, bool includePreRelease = false)
    {
        Name = name;
        Group = Group.None;
        IncludePreRelease = includePreRelease;
        Net9Version = null;
        Version = null;
    }
}

public static class PreDefinedLibrary
{
    public static List<Library> InitCommunityToolkit()
    {
        List<Library> list = new()
        {
            new Library("CommunityToolkit.HighPerformance", Group.CommunityToolkit),
            new Library("CommunityToolkit.Common", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Behaviors", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Extensions", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Helpers", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Triggers", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Converters", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Animations", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Media", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Collections", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Lottie", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.Segmented", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.Primitives", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.Sizers", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.HeaderedControls", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.RangeSelector", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.ImageCropper", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.RichSuggestBox", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.RadialGauge", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.CameraPreview", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.TokenizingTextBox", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.LayoutTransformControl", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.ColorPicker", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.TabbedCommandBar", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.SettingsControls", Group.CommunityToolkit),
            new Library("CommunityToolkit.WinUI.Controls.MetadataControl", Group.CommunityToolkit)
        };
        return list.OrderBy(x => x.Name).ToList();
    }

    public static List<Library> InitEFCore()
    {
        List<Library> list = new()
        {
            new Library("Microsoft.EntityFrameworkCore", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Sqlite", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.SqlServer", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Cosmos", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.InMemory", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Relational", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Abstractions", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Analyzers", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Design", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Proxies", Group.EntityFrameworkCore),
            new Library("Microsoft.EntityFrameworkCore.Tools", Group.EntityFrameworkCore)
        };
        return list.OrderBy(x => x.Name).ToList();
    }

    public static List<Library> InitUseful()
    {
        List<Library> list = new()
        {
            new Library("WinUI.Dock", Group.General),
            new Library("nucs.JsonSettings", Group.General),
            new Library("nucs.JsonSettings.AutosaveGenerator", Group.General),
            new Library("ComputeSharp.WinUI", Group.General),
            new Library("ComputeSharp.D2D1.WinUI", Group.General),
            new Library("Config.Net", Group.General),
            new Library("messagepack", Group.General),
            new Library("NotifyIconEx", Group.General),
            new Library("Ulid", Group.General),
            new Library("TenMica", Group.General, true),
            new Library("WinUI.TableView", Group.General),
            new Library("Microsoft.Windows.CsWinRT", Group.General),
            new Library("Microsoft.Windows.CsWin32", Group.General),
            new Library("WinUIEx", Group.General),
            new Library("Microsoft.Graphics.Win2D", Group.General),
            new Library("Newtonsoft.Json", Group.General),
            new Library("HtmlAgilityPack", Group.General),
            new Library("Downloader", Group.General),
            new Library("Microsoft.Win32.Registry", Group.General),
            new Library("YamlDotNet", Group.General),
            new Library("System.Drawing.Common", Group.General),
            new Library("System.Management", Group.General),
            new Library("SharpCompress", Group.General),
            new Library("RestSharp", Group.General),
            new Library("Vanara.Windows.Shell", Group.General),
            new Library("protobuf-net", Group.General),
            new Library("protobuf-net.Core", Group.General),
            new Library("Humanizer.Core", Group.General),
            new Library("LiveChartsCore.SkiaSharpView.WinUI", Group.General, true),
        };
        return list.OrderBy(x => x.Name).ToList();
    }

    public static List<Library> InitTest()
    {
        List<Library> list = new()
        {
            new Library("MSTest.TestAdapter", Group.Test),
            new Library("MSTest.TestFramework", Group.Test),
            new Library("Microsoft.TestPlatform.TestHost", Group.Test)
        };
        return list.OrderBy(x => x.Name).ToList();
    }
    public static List<Library> InitDevWinUI()
    {
        List<Library> list = new()
        {
            new Library(Constants.DevWinUI_Core, Group.DevWinUI),
            new Library(Constants.DevWinUI_Controls, Group.DevWinUI),
            new Library(Constants.DevWinUI_ContextMenu, Group.DevWinUI)
        };
        return list;
    }

    public static List<Library> InitLog()
    {
        List<Library> list = new()
        {
            new Library("Serilog", Group.Log),
            new Library("Serilog.Sinks.File", Group.Log),
            new Library("Serilog.Sinks.Debug", Group.Log),
            new Library("Serilog.Sinks.Console", Group.Log),
            new Library("log4net", Group.Log),
            new Library("NLog", Group.Log)
        };
        return list.OrderBy(x => x.Name).ToList();
    }

    public static List<Library> InitMVVM()
    {
        List<Library> list = new()
        {
            new Library("CommunityToolkit.Mvvm", Group.MVVM),
            new Library("Microsoft.Xaml.Behaviors.WinUI.Managed", Group.MVVM),
            new Library("Microsoft.Extensions.Hosting", Group.MVVM),
            new Library("Microsoft.Extensions.DependencyInjection", Group.MVVM),
            new Library("Microsoft.Extensions.Logging", Group.MVVM),
            new Library("Microsoft.Extensions.Configuration", Group.MVVM)
        };
        return list.OrderBy(x => x.Name).ToList();
    }
}
