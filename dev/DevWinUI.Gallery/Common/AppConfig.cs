using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace DevWinUIGallery.Common;

[GenerateAutoSaveOnChange]
public partial class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("8.5.0.0")]
    public Version Version { get; set; } = new Version(8, 5, 0, 0);

    public string fileName { get; set; } = Constants.AppConfigPath;
    public string lastUpdateCheck { get; set; }
}
