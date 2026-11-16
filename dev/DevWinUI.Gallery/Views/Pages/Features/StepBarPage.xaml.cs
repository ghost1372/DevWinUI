namespace DevWinUIGallery.Views;

public sealed partial class StepBarPage : Page
{
    public StepBarPage()
    {
        this.InitializeComponent();
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
        StepBarSample.Next();
    }

    private void BtnPrev_Click(object sender, RoutedEventArgs e)
    {
        StepBarSample.Prev();
    }
}
