using Nucs.JsonSettings;
using Nucs.JsonSettings.Autosave;
using Nucs.JsonSettings.Modulation;

namespace DevWinUIGallery.Common;

[Autosave]
public partial class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("10.0.0.0")]
    public Version Version { get; set; } = new Version(10, 0, 0, 0);

    public override string FileName { get; set; } = Constants.AppConfigPath;
    public bool UseDeveloperMode { get; set; } = true;
    public string LastUpdateCheck { get; set; }
}
