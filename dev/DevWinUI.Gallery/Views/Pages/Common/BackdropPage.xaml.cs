namespace DevWinUIGallery.Views;

public sealed partial class BackdropPage : Page
{
    public BackdropPage()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var backdropWindow = new SampleBackdrop();
        backdropWindow.Activate();
    }
}
