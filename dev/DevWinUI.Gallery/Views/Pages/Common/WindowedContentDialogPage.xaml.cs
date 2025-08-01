namespace DevWinUIGallery.Views;

public sealed partial class WindowedContentDialogPage : Page
{
    public WindowedContentDialogPage()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        WindowedContentDialog dialog = new()
        {
            Title = txtTitle.Text,
            Content = txtContent.Text,
            PrimaryButtonText = "Primary",
            SecondaryButtonText = "Secondary  ",
            CloseButtonText = "Close",
            OwnerWindow = MainWindow.Instance,
            HasTitleBar = TGHasTitleBar.IsOn,
            IsResizable = TGIsResizable.IsOn
        };

        ContentDialogResult result = await dialog.ShowAsync(TGIsModal.IsOn);
        TxtResult.Text = result.ToString();
    }
}
