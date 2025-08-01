using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Nucs.JsonSettings;
using Nucs.JsonSettings.Fluent;
using Nucs.JsonSettings.Modulation;
using Nucs.JsonSettings.Modulation.Recovery;

namespace DevWinUIGallery.Common;
public static partial class AppHelper
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppConfig))]
    public static AppConfig Settings = JsonSettings.Configure<AppConfig>()
                               .WithRecovery(RecoveryAction.RenameAndLoadDefault)
                               .WithVersioning(VersioningResultAction.RenameAndLoadDefault)
                               .LoadNow();

    public static (string UniqueId, string SectionId) GetUniqueIdAndSectionId(object parameter)
    {
        var uniqueId = string.Empty;
        var sectionId = string.Empty;

        var dataGroup = parameter as DataGroup;
        var dataItem = parameter as DataItem;

        if (dataGroup != null)
        {
            uniqueId = dataGroup.UniqueId;
            sectionId = dataGroup.SectionId;
        }

        if (dataItem != null)
        {
            uniqueId = dataItem.UniqueId;
            sectionId = dataItem.SectionId;
        }

        return (uniqueId, sectionId);
    }

    [LibraryImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
}

