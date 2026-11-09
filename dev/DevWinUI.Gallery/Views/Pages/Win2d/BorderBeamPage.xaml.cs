using Windows.UI;

namespace DevWinUIGallery.Views;

public sealed record BeamPreset(string Name, Color From, Color To);

public sealed partial class BorderBeamPage : Page
{
    public IReadOnlyList<BeamPreset> Presets { get; } = new[]
    {
        new BeamPreset("Sunset", Microsoft.UI.ColorHelper.FromArgb(255, 0xFF, 0xAA, 0x40), Microsoft.UI.ColorHelper.FromArgb(255, 0x9C, 0x40, 0xFF)),
        new BeamPreset("Ocean", Microsoft.UI.ColorHelper.FromArgb(255, 0x38, 0xBD, 0xF8), Microsoft.UI.ColorHelper.FromArgb(255, 0x63, 0x66, 0xF1)),
        new BeamPreset("Aurora", Microsoft.UI.ColorHelper.FromArgb(255, 0x34, 0xD3, 0x99), Microsoft.UI.ColorHelper.FromArgb(255, 0x3B, 0x82, 0xF6)),
        new BeamPreset("Ember", Microsoft.UI.ColorHelper.FromArgb(255, 0xFA, 0xCC, 0x15), Microsoft.UI.ColorHelper.FromArgb(255, 0xEF, 0x44, 0x44)),
        new BeamPreset("Candy", Microsoft.UI.ColorHelper.FromArgb(255, 0xF4, 0x72, 0xB6), Microsoft.UI.ColorHelper.FromArgb(255, 0x8B, 0x5C, 0xF6)),
    };

    public BorderBeamPage()
    {
        InitializeComponent();
    }

    public CornerRadius ToCornerRadius(double value) => new(value);
}
