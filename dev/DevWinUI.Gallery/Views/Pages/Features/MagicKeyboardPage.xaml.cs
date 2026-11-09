namespace DevWinUIGallery.Views;

public sealed partial class MagicKeyboardPage : Page
{
    public ObservableCollection<MagicKeyboardSkin> SkinItems { get; set; } = new ObservableCollection<MagicKeyboardSkin>(Enum.GetValues<MagicKeyboardSkin>());
    public ObservableCollection<MagicKeyboardLayout> LayoutItems { get; set; } = new ObservableCollection<MagicKeyboardLayout>(Enum.GetValues<MagicKeyboardLayout>());
    public ObservableCollection<MagicKeyboardFormFactor> FormFactorItems { get; set; } = new ObservableCollection<MagicKeyboardFormFactor>(Enum.GetValues<MagicKeyboardFormFactor>());
    public ObservableCollection<MagicKeyboardPlatform> PlatformItems { get; set; } = new ObservableCollection<MagicKeyboardPlatform>(Enum.GetValues<MagicKeyboardPlatform>());
    public ObservableCollection<MagicKeyboardKeyCase> LetterCaseItems { get; set; } = new ObservableCollection<MagicKeyboardKeyCase>(Enum.GetValues<MagicKeyboardKeyCase>());
    public ObservableCollection<MagicKeyboardKeyModifierMode> ModifierModeItems { get; set; } = new ObservableCollection<MagicKeyboardKeyModifierMode>(Enum.GetValues<MagicKeyboardKeyModifierMode>());

    public MagicKeyboardPage()
    {
        InitializeComponent();
    }

    private void Keyboard_KeyEvent(MagicKeyboard sender, MagicKeyEventArgs e)
    {
        TxtLastKey.Text = $"{e.Code} {(e.IsDown ? "down" : "up")} via {e.Source}";
    }
}
