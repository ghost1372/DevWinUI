using System.Collections.Generic;

namespace DevWinUI_Template
{
    public class Library
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Net9Version { get; set; }
        public bool IncludePreRelease { get; set; }

        public Library(string name, string version, string net9Version, bool includePreRelease = false)
        {
            Name = name;
            Net9Version = net9Version;
            Version = version;
            IncludePreRelease = includePreRelease;
        }

        public Library(string name, bool includePreRelease = false)
        {
            Name = name;
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
                new Library("CommunityToolkit.HighPerformance"),
                new Library("CommunityToolkit.Common"),
                new Library("CommunityToolkit.WinUI.Behaviors"),
                new Library("CommunityToolkit.WinUI.Extensions"),
                new Library("CommunityToolkit.WinUI.Helpers"),
                new Library("CommunityToolkit.WinUI.Triggers"),
                new Library("CommunityToolkit.WinUI.Converters"),
                new Library("CommunityToolkit.WinUI.Animations"),
                new Library("CommunityToolkit.WinUI.Media"),
                new Library("CommunityToolkit.WinUI.Collections"),
                new Library("CommunityToolkit.WinUI.Lottie"),
                new Library("CommunityToolkit.WinUI.Controls.Segmented"),
                new Library("CommunityToolkit.WinUI.Controls.Primitives"),
                new Library("CommunityToolkit.WinUI.Controls.Sizers"),
                new Library("CommunityToolkit.WinUI.Controls.HeaderedControls"),
                new Library("CommunityToolkit.WinUI.Controls.RangeSelector"),
                new Library("CommunityToolkit.WinUI.Controls.ImageCropper"  ),
                new Library("CommunityToolkit.WinUI.Controls.RichSuggestBox"),
                new Library("CommunityToolkit.WinUI.Controls.RadialGauge"),
                new Library("CommunityToolkit.WinUI.Controls.CameraPreview"),
                new Library("CommunityToolkit.WinUI.Controls.TokenizingTextBox"),
                new Library("CommunityToolkit.WinUI.Controls.LayoutTransformControl"),
                new Library("CommunityToolkit.WinUI.Controls.ColorPicker"),
                new Library("CommunityToolkit.WinUI.Controls.TabbedCommandBar"),
                new Library("CommunityToolkit.WinUI.Controls.SettingsControls")
            };
            return list;
        }

        public static List<Library> InitEFCore()
        {
            List<Library> list = new()
            {
                new Library("Microsoft.EntityFrameworkCore"),
                new Library("Microsoft.EntityFrameworkCore.Sqlite"),
                new Library("Microsoft.EntityFrameworkCore.SqlServer"),
                new Library("Microsoft.EntityFrameworkCore.Cosmos"),
                new Library("Microsoft.EntityFrameworkCore.InMemory"),
                new Library("Microsoft.EntityFrameworkCore.Relational"),
                new Library("Microsoft.EntityFrameworkCore.Abstractions"),
                new Library("Microsoft.EntityFrameworkCore.Analyzers"),
                new Library("Microsoft.EntityFrameworkCore.Design"),
                new Library("Microsoft.EntityFrameworkCore.Proxies"),
                new Library("Microsoft.EntityFrameworkCore.Tools")
            };
            return list;
        }

        public static List<Library> InitUseful()
        {
            List<Library> list = new()
            {
                new Library("ComputeSharp.WinUI", true),
                new Library("ComputeSharp.D2D1.WinUI", true),
                new Library("Config.Net"),
                new Library("messagepack"),
                new Library("NotifyIconEx"),
                new Library("Ulid"),
                new Library("TenMica", true),
                new Library("WinUI.TableView"),
                new Library("Microsoft.Windows.CsWinRT"),
                new Library("Microsoft.Windows.CsWin32"),
                new Library("WinUIEx"),
                new Library("Microsoft.Graphics.Win2D"),
                new Library("Newtonsoft.Json"),
                new Library("HtmlAgilityPack"),
                new Library("Downloader"),
                new Library("Microsoft.Win32.Registry"),
                new Library("YamlDotNet"),
                new Library("System.Drawing.Common"),
                new Library("System.Management"),
                new Library("SharpCompress"),
                new Library("RestSharp"),
                new Library("Vanara.Windows.Shell"),
                new Library("protobuf-net"),
                new Library("protobuf-net.Core"),
                new Library("Humanizer.Core"),
                new Library("LiveChartsCore.SkiaSharpView.WinUI", true),
            };
            return list;
        }

        public static List<Library> InitTest()
        {
            List<Library> list = new()
            {
                new Library("MSTest.TestAdapter"),
                new Library("MSTest.TestFramework"),
                new Library("Microsoft.TestPlatform.TestHost")
            };
            return list;
        }
        public static List<Library> InitDevWinUI()
        {
            List<Library> list = new()
            {
                new Library(Constants.DevWinUI_Core),
                new Library(Constants.DevWinUI_Controls),
                new Library(Constants.DevWinUI_ContextMenu)
            };
            return list;
        }

        public static List<Library> InitLog()
        {
            List<Library> list = new()
            {
                new Library("Serilog"),
                new Library("Serilog.Sinks.File"),
                new Library("Serilog.Sinks.Debug"),
                new Library("Serilog.Sinks.Console"),
                new Library("log4net"),
                new Library("NLog")
            };
            return list;
        }

        public static List<Library> InitMVVM()
        {
            List<Library> list = new()
            {
                new Library("CommunityToolkit.Mvvm"),
                new Library("Microsoft.Xaml.Behaviors.WinUI.Managed"),
                new Library("Microsoft.Extensions.Hosting"),
                new Library("Microsoft.Extensions.DependencyInjection"),
                new Library("Microsoft.Extensions.Logging"),
                new Library("Microsoft.Extensions.Configuration")
            };
            return list;
        }
    }
}
