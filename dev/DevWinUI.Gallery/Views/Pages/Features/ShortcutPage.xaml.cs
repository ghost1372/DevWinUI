namespace DevWinUIGallery.Views;

public sealed partial class ShortcutPage : Page
{
    public ShortcutPage()
    {
        InitializeComponent();

        MainShortcut.Keys = new List<object>() { "Win", "Alt", "F1" };
        MainShortcutPreview.Keys = new List<object>() { "Win", "Alt", "F1" };
        MainShortcutWithTextLabel.Keys = new List<object>() { "Win", "Alt", "F1" };
    }

    private void OnMainShortcutPrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        MainShortcut.UpdatePreviewKeys();
        MainShortcut.CloseContentDialog();
        TxtResult.Text = "Primary button clicked!" + Environment.NewLine + string.Join(" + ", MainShortcut.Keys);
    }

    private void OnMainShortcutSecondaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        TxtResult.Text = "Secondary button clicked!";
    }

    private void OnMainShortcutCloseButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        TxtResult.Text = "Close button clicked!";
    }
}
