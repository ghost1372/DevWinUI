namespace DevWinUIGallery.Views;

public sealed partial class HoldToConfirmPage : Page
{
    public HoldToConfirmPage()
    {
        InitializeComponent();
    }

    private string GetGlyph(bool isOn) => isOn ? "\uE72E" : string.Empty;

    private void Sample_Confirm(HoldToConfirm sender, object? args)
    {
        StatusText.Text = $"Confirmed at {DateTime.Now:T}";
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        Sample.Reset();
        StatusText.Text = "Press and hold the button";
    }
}
