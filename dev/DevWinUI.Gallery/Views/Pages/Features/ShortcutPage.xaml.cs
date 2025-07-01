namespace DevWinUIGallery.Views;

public sealed partial class ShortcutPage : Page
{
    public ShortcutPage()
    {
        InitializeComponent();

        MainShortcutEditor.Keys = new List<object>() { "Win", "Alt", "F1" };
        MainShortcut.Keys = new List<object>() { "Win", "Alt", "F1" };
        MainShortcutWithTextLabel.Keys = new List<object>() { "Win", "Alt", "F1" };
    }

    private void OnMainShortcutEditorPrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        MainShortcutEditor.UpdatePreviewKeys();
        MainShortcutEditor.CloseContentDialog();
        TxtResult.Text = "Primary button clicked!" + Environment.NewLine + MainShortcutEditor.Keys.ToString();
    }

    private void OnMainShortcutEditorSecondaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        TxtResult.Text = "Secondary button clicked!";
    }

    private void OnMainShortcutEditorCloseButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        TxtResult.Text = "Close button clicked!";
    }
}
