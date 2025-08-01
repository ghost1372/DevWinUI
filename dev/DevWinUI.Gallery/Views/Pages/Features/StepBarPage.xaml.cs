namespace DevWinUIGallery.Views;

public sealed partial class StepBarPage : Page
{
    public BaseViewModel ViewModel { get; }
    public StepBarPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
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
