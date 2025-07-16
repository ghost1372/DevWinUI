using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace DevWinUIGallery.Common;

[GenerateAutoSaveOnChange]
public partial class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("8.5.0.0")]
    public Version Version { get; set; } = new Version(8, 5, 0, 0);

    private string fileName { get; set; } = Constants.AppConfigPath;
    private bool useDeveloperMode { get; set; } = true;
    private string lastUpdateCheck { get; set; }
}
