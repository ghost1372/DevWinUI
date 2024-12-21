namespace DevWinUIGallery.Views;

public sealed partial class ClockPage : Page
{
    public ClockPage()
    {
        this.InitializeComponent();
    }

    private void Clock_SelectedTimeChanged(object sender, DateTime e)
    {
        Txt.Text = e.ToLongTimeString();
    }
}
