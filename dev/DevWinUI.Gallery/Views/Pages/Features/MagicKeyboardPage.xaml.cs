namespace DevWinUIGallery.Views;

public sealed partial class MagicKeyboardPage : Page
{
    public MagicKeyboardPage()
    {
        InitializeComponent();
    }

    private void Keyboard_KeyEvent(MagicKeyboard sender, MagicKeyEventArgs e)
    {
        TxtLastKey.Text = $"{e.Code} {(e.IsDown ? "down" : "up")} via {e.Source}";
    }
}
