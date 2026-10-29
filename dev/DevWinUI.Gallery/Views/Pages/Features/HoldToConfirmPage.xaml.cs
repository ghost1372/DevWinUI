namespace DevWinUIGallery.Views;

public sealed partial class HoldToConfirmPage : Page
{
    public ObservableCollection<HoldToConfirmKind> KindItems { get; set; } = new ObservableCollection<HoldToConfirmKind>(Enum.GetValues<HoldToConfirmKind>());
    public ObservableCollection<HoldToConfirmSize> SizeItems { get; set; } = new ObservableCollection<HoldToConfirmSize>(Enum.GetValues<HoldToConfirmSize>());

    public HoldToConfirmPage()
    {
        InitializeComponent();
    }

    private static string GetGlyph(bool isOn) => isOn ? "\uE72E" : string.Empty;

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
