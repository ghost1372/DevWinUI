namespace DevWinUIGallery.Views;

public sealed partial class HaloPage : Page
{
    public HaloPage()
    {
        InitializeComponent();
    }

    private void HaloTimePicker_TimeChanged(object sender, EventArgs e)
    {
        TxtTime.Text = ((HaloTimePicker)sender).Time.ToString(@"hh\:mm");
    }
}
