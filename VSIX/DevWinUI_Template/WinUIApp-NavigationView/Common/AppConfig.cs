using Nucs.JsonSettings;
using Nucs.JsonSettings.Modulation;
using Nucs.JsonSettings.Examples;

namespace $safeprojectname$.Common;

[GenerateAutoSaveOnChange]
public partial class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("1.0.0.0")]
    public Version Version { get; set; } = new Version(1, 0, 0, 0);

    public string fileName { get; set; } = Constants.AppConfigPath;
    $DeveloperModeConfig$$AppUpdateConfig$

    // Docs: https://github.com/Nucs/JsonSettings
}
