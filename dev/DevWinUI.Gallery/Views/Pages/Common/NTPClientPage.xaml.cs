namespace DevWinUIGallery.Views;

public sealed partial class NTPClientPage : Page
{
    public NTPClientPage()
    {
        InitializeComponent();
    }

    private async void BtnGet_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(TxtServer.Text))
            return;

        try
        {
            using var client = new NtpClient(TxtServer.Text);
            var response = await client.QueryAsync();

            TxtTime.Text = $"Server time: {response.TransmitTimestamp}";
            TxtOffset.Text = $"Clock offset: {response.ClockOffset}";
            TxtDelay.Text = $"Round-trip delay: {response.RoundTripDelay}";
            TxtStratum.Text = $"Stratum: {response.Stratum}";
        }
        catch (Exception ex)
        {
            await MessageBox.ShowErrorAsync(ex.Message);
        }
    }
}
