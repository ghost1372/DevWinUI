namespace DevWinUIGallery.Views;

public sealed partial class ShortcutGuidePage : Page
{
    private Shortcut c = new Shortcut();
    private ContentDialog shortcutDialog;
    bool canClose = false;
    public ShortcutGuidePage()
    {
        this.InitializeComponent();
        HotkeyMicVidControl.Keys = new List<object> { "Ctrl", "Alt", "F5" };
        HotkeyMicControl.Keys = new List<object> { "Ctrl", "Alt", "F5" };
        HotkeyVidControl.Keys = new List<object> { "Ctrl", "Alt", "F5" };
    }
    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var c = new Shortcut
        {
            Keys = new List<object> { "Ctrl", "Alt", "F5" }
        };

        shortcutDialog = new ContentDialog
        {
            XamlRoot = Content.XamlRoot,
            Title = "Activation shortcut",
            Content = c,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Confirm",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        shortcutDialog.Closing += ShortcutDialog_Closing;
        shortcutDialog.PrimaryButtonClick += ShortcutDialog_PrimaryButtonClick;
        shortcutDialog.SecondaryButtonClick += ShortcutDialog_SecondaryButtonClick;
        shortcutDialog.CloseButtonClick += ShortcutDialog_CloseButtonClick;

        await shortcutDialog.ShowAsync();
    }


    private void ShortcutDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        canClose = true;
    }

    private void ShortcutDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        args.Cancel = !canClose;
    }
    private void ShortcutDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        DisableKeys();
    }
    private void ShortcutDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        EnableKeys();
    }
    private void EnableKeys()
    {
        shortcutDialog.IsPrimaryButtonEnabled = true;
        c.IsError = false;
    }
    private void DisableKeys()
    {
        shortcutDialog.IsPrimaryButtonEnabled = false;
        c.IsError = true;
    }
}
